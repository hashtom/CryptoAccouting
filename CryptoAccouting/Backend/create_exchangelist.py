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
    coins = pd.read_json(json.dumps(bittrex_dict["result"]), orient='records')
    #coins.set_index('Currency', inplace=True)
    coins.drop(coins[coins.IsActive != True].index)
    symbols = pd.DataFrame(coins['Currency'])
    symbols.rename(columns={'Currency':'symbol'}, inplace=True)
    jsondata = symbols.to_json(orient='records')
else:
    jsondata = ""
    
#json.dump(jsondata, open(basedir + 'test.json', 'w',encoding='utf-8'),indent=4)
f = open(basedir + 'test.json', 'w',encoding='utf-8') # 書き込みモードで開く
f.write(jsondata) 
f.close() 