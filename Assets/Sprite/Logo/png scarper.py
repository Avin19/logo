import os
import time
import requests
from bs4 import BeautifulSoup
from urllib.parse import urljoin
from PIL import Image
from io import BytesIO

# ============================
# CONFIG
# ============================
BASE_URL = "https://pngimg.com/"
BASE_DOMAIN = "pngimg.com"
HEADERS = {"User-Agent": "Mozilla/5.0"}

ROOT_FOLDER = "Assets/Sprites"

MAX_SIZE = (256, 256)   # resize for mobile optimization

# ============================
# UTILS
# ============================
def get_soup(url):
    try:
        res = requests.get(url, headers=HEADERS, timeout=10)
        res.raise_for_status()
        return BeautifulSoup(res.text, "html.parser")
    except Exception:
        print(f"❌ Failed: {url}")
        return None


def clean_name(name):
    return (
        name.lower()
        .replace(" ", "_")
        .replace(",", "")
        .replace("-", "_")
        .strip()
    )

# ============================
# GET ALL SUBCATEGORIES
# ============================
def get_all_subcategories():
    soup = get_soup(BASE_URL)
    data = []

    if not soup:
        return data

    for block in soup.select("li.catalog"):
        category = clean_name(block.select_one("div.category a").text)

        for a in block.select("div.sub_category a"):
            href = a.get("href")

            if href:
                sub_name = clean_name(a.text)
                full_url = urljoin(BASE_URL, href)

                data.append((category, sub_name, full_url))

    return data

# ============================
# GET IMAGE PAGES
# ============================
def get_image_pages(sub_url):
    soup = get_soup(sub_url)
    if not soup:
        return []

    links = []

    for a in soup.select("div.png_png a"):
        href = a.get("href")

        if href and BASE_DOMAIN in href:
            links.append(href)

    return list(set(links))

# ============================
# GET PNG LINK
# ============================
def get_png(img_page):
    soup = get_soup(img_page)

    if not soup:
        return None

    img = soup.select_one("img[itemprop='contentUrl']")
    if img:
        return img.get("src")

    return None

# ============================
# DOWNLOAD + COMPRESS
# ============================
def download_image(img_page, folder, sub_name):
    png_url = get_png(img_page)

    if not png_url:
        return

    filename = f"{sub_name}.png"
    path = os.path.join(folder, filename)

    # Skip if already downloaded
    if os.path.exists(path):
        print(f"⏩ Skipped {filename}")
        return

    try:
        res = requests.get(png_url, headers=HEADERS, timeout=10)

        # Load image
        img = Image.open(BytesIO(res.content)).convert("RGBA")

        # Resize (important)
        img.thumbnail(MAX_SIZE)

        # Save optimized PNG
        img.save(path, format="PNG", optimize=True)

        print(f"✅ {filename}")

    except Exception as e:
        print(f"❌ Error: {png_url} | {e}")

# ============================
# MAIN PIPELINE
# ============================
def run():
    print("🔍 Fetching categories...")
    data = get_all_subcategories()

    print(f"📦 Total subcategories: {len(data)}\n")

    for category, sub_name, sub_url in data:
        folder = os.path.join(ROOT_FOLDER, category)
        os.makedirs(folder, exist_ok=True)

        print(f"\n📂 {category} → {sub_name}")

        image_pages = get_image_pages(sub_url)

        if not image_pages:
            continue

        # ✅ Take ONLY first image
        first_image = image_pages[0]

        download_image(first_image, folder, sub_name)

        time.sleep(0.3)  # be polite to server

# ============================
# ENTRY
# ============================
if __name__ == "__main__":
    run()