#!/usr/local/bin/python
# -*- coding: utf-8 -*-

import urllib

print "Content-Type: application/json\n"

baseurl = "https://api.coinmarketcap.com"
response = urllib.urlopen(baseurl + "/v1/ticker/?limit=0")
json = response.read()
print(json)