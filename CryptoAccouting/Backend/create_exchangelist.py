#!/usr/bin/env python3
# -*- coding: utf-8 -*-
import json
import requests
import re
import pandas as pd

basedir = "/Users/name/Downloads/"

exchanges = []
ExchangeList = {"exchanges": exchanges}

#BitStamp
bitstamp_dict = {
        "code": "Bitstamp",
        "name": "Bitstamp",
        "apiprice": "true",
        "apitrade": "true",
        "apibalance": "true",
        "calcPL" : "False",
        "listing": [ 
                { "symbol": "BTC" },
                { "symbol": "XRP" }
                ]
        }
exchanges.append(bitstamp_dict)

# Bittrex
bittrex_baseurl = "https://bittrex.com"
bittrex = requests.get(bittrex_baseurl + "/api/v1.1/public/getcurrencies").json()
bittrex_dict = { "code": "Bittrex",
                "name": "Bittrex",
                "apiprice": "true",
                "apitrade": "true",
                "apibalance": "true",
                "calcPL" : "False"
                }

if bittrex["success"] == True:
    df = pd.read_json(json.dumps(bittrex["result"]), orient='records')
    #df.drop(df[df.IsActive != True].index, inplace=True)
    bittrex_coins = pd.DataFrame(df['Currency'])
    bittrex_coins.rename(columns={'Currency':'symbol'}, inplace=True)
    bittrex_coins.drop(bittrex_coins[bittrex_coins.symbol=="BCC"].index, inplace=True)
    bittrex_coins.drop(bittrex_coins[bittrex_coins.symbol=="IOTA"].index, inplace=True)
    symbols = bittrex_coins.to_dict(orient='records')
    symbols.append({"symbol2" : "BCC"})
    symbols.append({"symbol2" : "IOTA"})
    bittrex_dict["listing"] = symbols
    exchanges.append(bittrex_dict)

#GDAX
gdax_dict = { 
        "code": "GDAX",
        "name": "GDAX",
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
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
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
         "listing": [
                 { "symbol": "BTC" },
                 { "symbol": "ETH" }
                 ]
         }
exchanges.append(gemini_dict)

#Kraken
#kraken = requests.get("https://api.kraken.com/0/public/Assets").json()
#kraken_dict = {"code": "Kraken",
#                 "name": "Kraken",
#                 "apiprice": "true",
#                 "apitrade": "false",
#                 "apibalance": "false"
#                 }
#if kraken['error'] == []:
#    df = pd.read_json(json.dumps(kraken["result"]), orient='index')
#    df.drop(df[df.index == 'XDAO'].index,inplace=True)
#    df.drop(df[df.index == 'KFEE'].index,inplace=True)
#    df.drop(df[df.index == 'ZCAD'].index,inplace=True)
#    df.drop(df[df.index == 'ZEUR'].index,inplace=True)
#    df.drop(df[df.index == 'ZGBP'].index,inplace=True)
#    df.drop(df[df.index == 'ZJPY'].index,inplace=True)
#    df.drop(df[df.index == 'ZKRW'].index,inplace=True)
#    df.drop(df[df.index == 'ZUSD'].index,inplace=True)
#    df.drop(df[df.index == 'XBT'].index,inplace=True)
#    df.drop(df[df.index == 'XDG'].index,inplace=True)
#    df.drop(df[df.index == 'VEN'].index,inplace=True)
#    kraken_symbols = pd.DataFrame({"symbol":list(df.altname)})
#    symbols = kraken_symbols.to_dict(orient='records')
#    symbols.append({"symbol2" : "XBT"})
#    symbols.append({"symbol2" : "XDG"})
#    kraken_dict["listing"] = symbols
#    exchanges.append(kraken_dict)
    
kraken_dict = {
        "code": "Kraken",
        "name": "Kraken",
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
        "listing": [
                { "symbol": "BCH" },
                { "symbol": "DASH" },
                { "symbol": "EOS" },
                { "symbol": "GNO" },
                { "symbol": "USDT" },
                { "symbol": "ETC" },
                { "symbol": "ETH" },
                { "symbol": "ICN" },
                { "symbol": "LTC" },
                { "symbol": "MLN" },
                { "symbol": "NMC" },
                { "symbol": "REP" },
                { "symbol2": "XBT" },
                { "symbol2": "XDG" },
                { "symbol": "XLM" },
                { "symbol": "XMR" },
                { "symbol": "XRP" },
                ]
        }
exchanges.append(kraken_dict)

#BitFinex
bitfinex = requests.get("https://api.bitfinex.com/v1/symbols").json()
bitfinex_dict = {"code": "Bitfinex",
                 "name": "Bitfinex",
                 "apiprice": "true",
                 "apitrade": "false",
                 "apibalance": "false",
                 "calcPL" : "False"
                 }
bitfinex_symbols = [] #pd.Series()
for ins in bitfinex:
    if re.search(r"\w\w\wusd",ins) or re.search(r"\w\w\wbtc",ins) :
        bitfinex_symbols.append(ins[0:3].upper())

bitfinex_symbols = pd.DataFrame({"symbol":bitfinex_symbols})
bitfinex_symbols.drop_duplicates(inplace=True)
bitfinex_symbols.drop(bitfinex_symbols[bitfinex_symbols.symbol=="IOT"].index, inplace=True)
symbols = bitfinex_symbols.to_dict(orient='records')
symbols.append({"symbol2" : "IOT"})
bitfinex_dict["listing"] = symbols

exchanges.append(bitfinex_dict)

#Poloniex
poloniex = requests.get("https://poloniex.com/public?command=returnCurrencies").json()
poloniex_dict = {"code": "Poloniex",
                 "name": "Poloniex",
                 "apiprice": "true",
                 "apitrade": "true",
                 "apibalance": "true",
                 "calcPL" : "False"
                 }
df_poloniex = pd.DataFrame.from_dict(poloniex, orient='index')
df_poloniex.drop(df_poloniex[df_poloniex.delisted==1].index, inplace=True)
poloniex_symbols = pd.DataFrame({"symbol":list(df_poloniex.index)})
poloniex_symbols.drop_duplicates(inplace=True)
poloniex_symbols.drop(poloniex_symbols[poloniex_symbols.symbol=="STR"].index, inplace=True)
symbols = poloniex_symbols.to_dict(orient='records')
symbols.append({"symbol2" : "STR"})
poloniex_dict["listing"] = symbols
#poloniex_dict["listing"] = poloniex_symbols.to_dict(orient='records')

exchanges.append(poloniex_dict)

#Zaif
zaif_dict = { 
        "code": "Zaif",
        "name": "Zaif",
        "apiprice": "true",
        "apitrade": "true",
        "apibalance": "true",
        "calcPL" : "true",
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
        "code": "bitFlyer_l",
        "name": "bitFlyer(lightn",
        "apiprice": "true",
        "apitrade": "true",
        "apibalance": "true",
        "calcPL" : "true",
         "listing": [
                 { "symbol": "BTC" }
                 ]
         }
exchanges.append(bitflyer_dict)

bitflyer_otc_dict = { 
        "code": "bitFlyer",
        "name": "bitFlyer",
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
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
        "apiprice": "true",
        "apitrade": "true",
        "apibalance": "true",
        "calcPL" : "true",
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

#BTCBOX
btcbox_dict = { 
        "code": "BTCBox",
        "name": "BTCBox",
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
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
        "apiprice": "true",
        "apitrade": "true",
        "apibalance": "true",
        "calcPL" : "true",
         "listing": [
                 { "symbol": "BTC" },
                 { "symbol": "BCH" },
                 { "symbol": "ETH" },
                 ]
         }
exchanges.append(quoine_dict)

#Bitbank
bitbank_dict = { 
        "code": "Bitbank",
        "name": "Bitbank",
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
         "listing": [
                 { "symbol": "BTC" },
                 { "symbol": "BCH" }
                 ]
         }
exchanges.append(bitbank_dict)
 
#Bithumb
bithumb_dict = {
        "code": "Bithumb",
        "name": "Bithumb",
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
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

#HitBTC
hitbtc = requests.get("https://api.hitbtc.com/api/2/public/currency").json()
hitbtc_dict = {"code": "HitBTC",
                 "name": "HitBTC",
                 "apiprice": "true",
                 "apitrade": "false",
                 "apibalance": "false",
                 "calcPL" : "False"
                 }
df_hitbtc = pd.DataFrame.from_dict(hitbtc)
df_hitbtc.drop(df_hitbtc[df_hitbtc.crypto==False].index, inplace=True)
hitbtc_symbols = pd.DataFrame({"symbol":list(df_hitbtc.id)})
#hitbtc_symbols.drop(hitbtc_symbols[hitbtc_symbols.symbol=="STR"].index, inplace=True)
symbols = hitbtc_symbols.to_dict(orient='records')
#symbols.append({"symbol2" : "STR"})
hitbtc_dict["listing"] = symbols
exchanges.append(hitbtc_dict)

#Cryptopia
cryptopia = requests.get("https://www.cryptopia.co.nz/api/GetCurrencies").json()
cryptopia_dict = {"code": "Cryptopia",
                 "name": "Cryptopia",
                 "apiprice": "false",
                 "apitrade": "false",
                 "apibalance": "false",
                 "calcPL" : "False",
                 }

if cryptopia['Success'] == True:
    df = pd.read_json(json.dumps(cryptopia["Data"]), orient='records')
    df.drop(df[df.ListingStatus!="Active"].index, inplace=True)
    cryptopia_symbols = pd.DataFrame({"symbol":list(df.Symbol)})
    cryptopia_symbols.drop(cryptopia_symbols[cryptopia_symbols.symbol=="BTG"].index, inplace=True)
    symbols = cryptopia_symbols.to_dict(orient='records')
    symbols.append({"symbol2" : "BTG"})
    cryptopia_dict["listing"] = symbols
    exchanges.append(cryptopia_dict)

#Binance
binance = requests.get("https://api.binance.com/api/v1/ticker/allPrices").json()
binance_dict = {"code": "Binance",
                 "name": "Binance",
                 "apiprice": "true",
                 "apitrade": "true",
                 "apibalance": "true",
                 "calcPL" : "False"
                 }

df = pd.read_json(json.dumps(binance))
binance_symbols = pd.DataFrame(df['symbol'])
binance_symbols = binance_symbols[binance_symbols.symbol.str.contains('BTC')]
binance_symbols = binance_symbols.symbol.str.replace('BTC', '')
binance_symbols = pd.DataFrame(binance_symbols)
binance_symbols.drop(binance_symbols[binance_symbols.symbol=="IOTA"].index, inplace=True)
binance_symbols.drop(binance_symbols[binance_symbols.symbol=="BCC"].index, inplace=True)
symbols = binance_symbols.to_dict(orient='records')
symbols.append({"symbol2" : "IOTA"})
symbols.append({"symbol2" : "BCC"})
symbols.append({"symbol" : "BTC"})
binance_dict["listing"] = symbols
exchanges.append(binance_dict)

#OTC
otc_dict = { 
        "code": "OTC",
        "name": "Over The Counter",
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
        "listing": []
        }

exchanges.append(otc_dict)

#AirDrop
airdrop_dict = {
          "code": "AirDrop",
          "name": "Air Drop",
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
          "listing": []
        }
exchanges.append(airdrop_dict)
    
#Other
other_dict = { 
        "code": "Other",
        "name": "Other Exchange",
        "apiprice": "false",
        "apitrade": "false",
        "apibalance": "false",
        "calcPL" : "False",
        "listing": []
        }
exchanges.append(other_dict)

# Create File
json.dump(ExchangeList, open(basedir + 'ExchangeList.json', 'w',encoding='utf-8'),indent=4)

