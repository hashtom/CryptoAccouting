#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import json
import requests
import pandas as pd

bittrex_baseurl = "https://bittrex.com"
#basedir = "/home/bridgeplace/www/coinbalance/"
basedir = "/Users/tomoaki/"

bittrex_dict = requests.get(bittrex_baseurl + "/api/v1.1/public/getcurrencies").json()
#bittrex_text = requests.get(bittrex_baseurl + "/api/v1.1/public/getcurrencies").text

if bittrex_dict["success"] == True:
    coins = pd.read_json(json.dumps(bittrex_dict["result"]))
    coins.drop(coins[coins.IsActive != True].index)
    coins = coins["Currency"]
    json = coins.to_json()
else:
    json = ""
