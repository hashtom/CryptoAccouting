#!/bin/sh
PATH=/bin:/usr/bin:/sbin:/usr/sbin:/usr/local/bin

stamp=`/bin/date +"%Y%m%d%H"`
yesterday=`/bin/date -v-1d +"%Y%m%d%H"`

work="/home/bridgeplace/www/cryptoticker/fxrate"

#wget -i $work/url.txt -O $work/data/fxrate_"$stamp".json -o $work/fxrate.log
wget "https://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote?format=json" -O $work/data/fxrate_"$stamp".json -o $work/fxrate.log

cp -p $work/data/fxrate_"$stamp".json $work/fxrate_latest.json
cp -p $work/data/fxrate_"$yesterday".json $work/fxrate_yesterday.json