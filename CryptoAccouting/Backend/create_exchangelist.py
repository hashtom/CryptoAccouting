#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import json
import requests
import re
import pandas as pd

#basedir = "/home/bridgeplace/scripts/"
basedir = "/Users/name/"

exchanges = []
ExchangeList = {"exchanges": exchanges}

#Other
other_dict = { 
        "code": "Other",
        "name": "Other Exchange",
        "api": "false",
        "listing": []
        }
exchanges.append(other_dict)
       
#Zaif
zaif_dict = { 
        "code": "Zaif",
        "name": "Zaif",
        "api": "true",
        "listing": [
             { "symbol": "BTC" },
             { "symbol": "MONA" },
             { "symbol": "XEM" },
             { "symbol": "ETH" },
             { "symbol": "BCH" }
             ]
        }
exchanges.append(zaif_dict)

#BitFlyer
bitflyer_dict = { 
        "code": "BitFlyer",
        "name": "BitFlyer",
        "api": "true",
         "listing": [
                 { "symbol": "BTC" }
                 ]
         }
exchanges.append(bitflyer_dict)

bitflyer_otc_dict = { 
        "code": "BitFlyer_OTC",
        "name": "BitFlyer(OTC)",
        "api": "false",
         "listing": [
                 { "symbol": "BTC" },
                 { "symbol": "LTC" },
                 { "symbol": "ETH" },
                 { "symbol": "ETC" },
                 { "symbol": "MONA" },
                 { "symbol": "BCH" }
                 ]
         }
exchanges.append(bitflyer_otc_dict)

#CoinCheck
coincheck_dict = {
        "code": "CoinCheck",
        "name": "CoinCheck",
        "api": "true",
        "listing": [
                { "symbol": "BTC" },
                { "symbol": "ETH" },
                { "symbol": "BCH" },
                { "symbol": "ETC" },
                { "symbol": "LSK" },
                { "symbol": "FCT" },
                { "symbol": "XMR" },
                { "symbol": "REP" },
                { "symbol": "XRP" },
                { "symbol": "ZEC" },
                { "symbol": "XEM" },
                { "symbol": "LTC" },
                { "symbol": "DASH" }
          ]
        }
exchanges.append(coincheck_dict)

#BitStamp
bitstamp_dict = {
        "code": "Bitstamp",
        "name": "Bitstamp",
        "api": "false",
        "listing": [ 
                { "symbol": "BTC" },
                { "symbol": "XRP" }
                ]
        }
exchanges.append(bitstamp_dict)

# Bittrex
bittrex_baseurl = "https://bittrex.com"
bittrex = requests.get(bittrex_baseurl + "/api/v1.1/public/getcurrencies").json()
#bittrex_text = requests.get(bittrex_baseurl + "/api/v1.1/public/getcurrencies").text
bittrex_dict = { "code": "Bittrex",
                "name": "Bittrex",
                "api": "true"}
                #"listing": bittrex_coins}

if bittrex["success"] == True:
    df = pd.read_json(json.dumps(bittrex["result"]), orient='records')
    #coins.set_index('Currency', inplace=True)
    df.drop(df[df.IsActive != True].index, inplace=True)
    bittrex_coins = pd.DataFrame(df['Currency'])
    bittrex_coins.rename(columns={'Currency':'symbol'}, inplace=True)
    bittrex_coins.drop(bittrex_coins[bittrex_coins.symbol=="BCC"].index, inplace=True)
    symbols = bittrex_coins.to_dict(orient='records')
    symbols.append({"symbol2" : "BCC"})
    bittrex_dict["listing"] = symbols
    exchanges.append(bittrex_dict)

#BTCBOX
btcbox_dict = { 
        "code": "BTCBox",
        "name": "BTCBox",
        "api": "false",
         "listing": [
                 { "symbol": "BTC" },
                 { "symbol": "BCH" }
                 ]
         }
exchanges.append(btcbox_dict)

#QUOINE
quoine_dict = { 
        "code": "Quoine",
        "name": "Quoine",
        "api": "false",
         "listing": [
                 { "symbol": "BTC" },
                 { "symbol": "BCH" }
                 ]
         }
exchanges.append(quoine_dict)

#Bitbank
bitbank_dict = { 
        "code": "Bitbank",
        "name": "Bitbank",
        "api": "false",
         "listing": [
                 { "symbol": "BTC" },
                 { "symbol": "BCH" }
                 ]
         }
exchanges.append(bitbank_dict)
 
#GDAX
gdax_dict = { 
        "code": "GDAX",
        "name": "GDAX",
        "api": "false",
         "listing": [
                 { "symbol": "BTC" },
                 { "symbol": "LTC" },
                 { "symbol": "ETH" }
                 ]
         }
exchanges.append(gdax_dict)

#Gemini
gemini_dict = { 
        "code": "Gemini",
        "name": "Gemini",
        "api": "false",
         "listing": [
                 { "symbol": "BTC" },
                 { "symbol": "ETH" }
                 ]
         }
exchanges.append(gemini_dict)

#Kraken
kraken_dict = {
        "code": "Kraken",
        "name": "Kraken",
        "api": "false",
        "listing": [
                { "symbol": "BTC" },
                { "symbol": "BCH" },
                { "symbol": "ETH" },
                { "symbol": "XRP" },
                { "symbol": "LTC" },
                { "symbol": "ETC" },
                { "symbol": "DASH" },
                { "symbol": "XMR" },
                { "symbol": "ZEC" },
                { "symbol": "ICN" },
                { "symbol": "XLM" },
                { "symbol": "DOGE" },
                { "symbol": "REP" },
                { "symbol": "MLN" }
                ]
        }
exchanges.append(kraken_dict)

#Gatecoin
    

#BitFinex
bitfinex = requests.get("https://api.bitfinex.com/v1/symbols").json()
bitfinex_dict = {"code": "BitFinex",
                 "name": "BitFinex",
                 "api": "false"}
bitfinex_symbols = [] #pd.Series()
for ins in bitfinex:
    if re.search(r"\w\w\wusd",ins) or re.search(r"\w\w\wbtc",ins) :
        bitfinex_symbols.append(ins[0:3].upper())

bitfinex_symbols = pd.DataFrame({"symbol":bitfinex_symbols})
bitfinex_symbols.drop_duplicates(inplace=True)
bitfinex_dict["listing"] = bitfinex_symbols.to_dict(orient='records')

exchanges.append(bitfinex_dict)

#Poloniex
poloniex = requests.get("https://poloniex.com/public?command=returnCurrencies").json()
poloniex_dict = {"code": "Poloniex",
                 "name": "Poloniex",
                 "api": "false"}
poloniex_symbols = pd.DataFrame({"symbol":list(poloniex.keys())})
poloniex_symbols.drop_duplicates(inplace=True)
poloniex_dict["listing"] = poloniex_symbols.to_dict(orient='records')

exchanges.append(poloniex_dict)

#BitMEX

#Bithumb
bithumb_dict = {
        "code": "Bithumb",
        "name": "Bithumb",
        "api": "false",
        "listing": [
                { "symbol": "BTC" }, 
                { "symbol": "ETH" },
                { "symbol": "BCH" },
                { "symbol": "XRP" },
                { "symbol": "LTC" },
                { "symbol": "ETC" },
                { "symbol": "XMR" },
                { "symbol": "QTUM" },
                { "symbol": "DASH" },
                { "symbol": "ZEC" }
                ]
        }
exchanges.append(bithumb_dict)

#Coinone

#Korbit

#OTC
otc_dict = { 
        "code": "OTC",
        "name": "Over The Counter",
        "api": "false",
        "listing": []
        }

exchanges.append(otc_dict)

#AirDrop
airdrop_dict = {
          "code": "AirDrop",
          "name": "Air Drop",
          "api": "false",
          "listing": []
        }
exchanges.append(airdrop_dict)
    
# Create File
json.dump(ExchangeList, open(basedir + 'ExchangeList.json', 'w',encoding='utf-8'),indent=4)

