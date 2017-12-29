#!/bin/sh
PATH=/bin:/usr/bin:/sbin:/usr/sbin:/usr/local/bin

stamp=`/bin/date +"%Y%m%d%H"`
yesterday=`/bin/date -v-1d +"%Y%m%d%H"`

work="/home/bridgeplace/www/coinbalance/fxrate"
#work="/home/bridgeplace/scripts"
log="/home/bridgeplace/scripts"

size=0

while :
do
  wget "https://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote?format=json" -O $work/data/fxrate_"$stamp".json -o $log/fxrate.log
  size=`ls -l $work/data/fxrate_"$stamp".json | awk '{print $5}'`
  echo $size
  if [ $size > 1000 ]; then
    cp -p $work/data/fxrate_"$stamp".json $work/fxrate_latest.json
    cp -p $work/data/fxrate_"$yesterday".json $work/fxrate_yesterday.json
    echo "done!"
    break
  fi
done