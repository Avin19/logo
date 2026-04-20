import os
import time
import requests
from bs4 import BeautifulSoup
from urllib.parse import urljoin
from concurrent.futures import ThreadPoolExecutor

# ============================
# CONFIG
# ============================
BASE_URL = "https://pngimg.com/"
BASE_DOMAIN = "pngimg.com"
HEADERS = {"User-Agent": "Mozilla/5.0"}
MAX_WORKERS = 5

ROOT_FOLDER = "Assets/Sprites"


# ============================
# UTILS
# ============================
def get_soup(url):
    try:
        res = requests.get(url, headers=HEADERS, timeout=10)
        res.raise_for_status()
        return BeautifulSoup(res.text, "html.parser")
    except:
        return None


def clean_name(name):
    return name.lower().replace(" ", "_").replace(",", "").strip()


# ============================
# GET ALL SUBCATEGORIES
# ============================
def get_all_subcategories():
    soup = get_soup(BASE_URL)
    data = []

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
# DOWNLOAD WORKER (RENAMED)
# ============================
def download_worker(img_page, folder, category, sub_name, index):
    png_url = get_png(img_page)

    if not png_url:
        return

    filename = f"{category}_{sub_name}_{index:03}.png"
    path = os.path.join(folder, filename)

    if os.path.exists(path):
        return

    try:
        img_data = requests.get(png_url, headers=HEADERS, timeout=10).content

        with open(path, "wb") as f:
            f.write(img_data)

        print(f"✅ {filename}")

    except:
        print(f"❌ {png_url}")


# ============================
# MAIN PIPELINE
# ============================
def run():
    print("🔍 Fetching categories...")
    data = get_all_subcategories()

    print(f"📦 Total subcategories: {len(data)}\n")

    for category, sub_name, sub_url in data:
        folder = os.path.join(ROOT_FOLDER, category, sub_name)
        os.makedirs(folder, exist_ok=True)

        print(f"\n📂 {category}/{sub_name}")

        image_pages = get_image_pages(sub_url)
        print(f"   → {len(image_pages)} images")

        with ThreadPoolExecutor(max_workers=MAX_WORKERS) as executor:
            for i, img in enumerate(image_pages, start=1):
                executor.submit(
                    download_worker,
                    img,
                    folder,
                    category,
                    sub_name,
                    i
                )

        time.sleep(0.5)


# ============================
# ENTRY
# ============================
if __name__ == "__main__":
    run()