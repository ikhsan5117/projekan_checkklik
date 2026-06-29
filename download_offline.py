import os
import re
import urllib.request
from urllib.error import URLError

base_dir = r"d:\CheckingAM\wwwroot\lib\offline"
os.makedirs(base_dir, exist_ok=True)
os.makedirs(os.path.join(base_dir, "css"), exist_ok=True)
os.makedirs(os.path.join(base_dir, "js"), exist_ok=True)
os.makedirs(os.path.join(base_dir, "fonts"), exist_ok=True)

def download(url, dest):
    print(f"Downloading {url} to {dest}")
    req = urllib.request.Request(url, headers={'User-Agent': 'Mozilla/5.0'})
    try:
        with urllib.request.urlopen(req) as response, open(dest, 'wb') as out_file:
            data = response.read()
            out_file.write(data)
            return data
    except Exception as e:
        print(f"Failed to download {url}: {e}")
        return None

# JS files
js_files = {
    "phosphor-icons.js": "https://unpkg.com/phosphor-icons",
    "signalr.min.js": "https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js",
    "signalr-6.0.1.min.js": "https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/6.0.1/signalr.min.js",
    "chart.js": "https://cdn.jsdelivr.net/npm/chart.js",
    "sweetalert2.js": "https://cdn.jsdelivr.net/npm/sweetalert2@11"
}

for name, url in js_files.items():
    download(url, os.path.join(base_dir, "js", name))

# Fonts
def process_google_fonts(css_url, css_filename):
    req = urllib.request.Request(css_url, headers={'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36'})
    try:
        with urllib.request.urlopen(req) as response:
            css_content = response.read().decode('utf-8')
            
            # Find all font urls
            urls = re.findall(r'url\((https://fonts\.gstatic\.com/[^\)]+)\)', css_content)
            for url in set(urls):
                filename = url.split('/')[-1]
                download(url, os.path.join(base_dir, "fonts", filename))
                css_content = css_content.replace(url, f"../fonts/{filename}")
            
            with open(os.path.join(base_dir, "css", css_filename), 'w', encoding='utf-8') as f:
                f.write(css_content)
            print(f"Saved {css_filename}")
    except Exception as e:
        print(f"Error processing google fonts {css_url}: {e}")

process_google_fonts("https://fonts.googleapis.com/css2?family=Outfit:wght@300;400;500;600;700;800&family=Sora:wght@300;400;500;600;700&display=swap", "google-fonts-outfit-sora.css")
process_google_fonts("https://fonts.googleapis.com/css?family=Lato:300,400,700", "google-fonts-lato.css")

# Phosphor icons CSS
def process_phosphor(url_base, css_name, font_names):
    css_url = f"{url_base}/style.css"
    req = urllib.request.Request(css_url, headers={'User-Agent': 'Mozilla/5.0'})
    try:
        with urllib.request.urlopen(req) as response:
            css_content = response.read().decode('utf-8')
            
            for font_name in font_names:
                font_url = f"{url_base}/{font_name}"
                download(font_url, os.path.join(base_dir, "fonts", font_name))
                css_content = css_content.replace(f"./{font_name}", f"../fonts/{font_name}")
            
            with open(os.path.join(base_dir, "css", css_name), 'w', encoding='utf-8') as f:
                f.write(css_content)
    except Exception as e:
        print(f"Error processing phosphor {url_base}: {e}")

process_phosphor("https://unpkg.com/@phosphor-icons/web@2.0.3/src/regular", "phosphor-regular.css", ["Phosphor.woff2", "Phosphor.woff", "Phosphor.ttf", "Phosphor.svg"])
process_phosphor("https://unpkg.com/@phosphor-icons/web@2.0.3/src/fill", "phosphor-fill.css", ["Phosphor-Fill.woff2", "Phosphor-Fill.woff", "Phosphor-Fill.ttf", "Phosphor-Fill.svg"])

print("Done")
