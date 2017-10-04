#!/bin/sh
PATH=/bin:/usr/bin:/sbin:/usr/sbin:/usr/local/bin

stamp=`/bin/date +"%Y%m%d%H"`
yesterday=`/bin/date -v-1d +"%Y%m%d%H"`

work="/home/bridgeplace/www/cryptoticker/market"
log="/home/bridgeplace/scripts"

wget "https://api.coinmarketcap.com/v1/ticker/" -O $work/data/market_"$stamp".json -o $log/market.log

cp -p $work/data/market_"$stamp".json $work/market_latest.json
cp -p $work/data/market_"$yesterday".json $work/market_yesterday.json