﻿import json
import requests

url = "https://api.coinmarketcap.com/v1/ticker/?limit=0"
logo_url = "https://files.coinmarketcap.com/static/img/coins/32x32/"
basedir = "/home/bridgeplace/www/coinbalance/"

def download_image(url, timeout = 10):
    response = requests.get(url, allow_redirects=False, timeout=timeout)
    if response.status_code != 200:
        e = Exception("HTTP status: " + response.status_code)
        raise e

    content_type = response.headers["content-type"]
    if 'image' not in content_type:
        e = Exception("Content-Type: " + content_type)
        raise e

    return response.content

def save_image(filename, image):
    with open(filename, "wb") as fout:
        fout.write(image)

InstrumentList = requests.get(url).json()

data = []
del InstrumentList[500:]
for ins in InstrumentList:
        if ins["id"] == "bitcoin-cash":
            data.append({
                "active":"true",
                "rank":ins["rank"],
                "name":ins["name"],
                "symbol":ins["symbol"],
                "symbol2":"BCC",
                "id":ins["id"]
                })
        else:
            data.append({
                "active":"true",
                "rank":ins["rank"],
                "name":ins["name"],
                "symbol":ins["symbol"],
                "id":ins["id"]
                        })
        try:
            image = download_image(logo_url + ins["id"] + ".png")
            save_image(basedir + "/images/" + ins["id"] + ".png", image)
        except KeyboardInterrupt:
            break
        except Exception as err:
            print(err)

     
json.dump(data, open(basedir + 'InstrumentList.json', 'w',encoding='utf-8'),indent=4)