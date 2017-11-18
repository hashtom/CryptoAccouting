#!/bin/sh
PATH=/home/bridgeplace/.pyenv/shims:/home/bridgeplace/.pyenv/bin:/bin:/usr/bin:/sbin:/usr/sbin:/usr/local/bin

stamp=`/bin/date +"%Y%m%d%H"`

dev="/home/bridgeplace/www/coinbalance/develop"
prod="/home/bridgeplace/www/coinbalance/v1.1"

cp -p $dev/InstrumentList.json $dev/bkup/InstrumentList.json.$stamp
cp -p $dev/ExchangeList.json $dev/bkup/ExchangeList.json.$stamp

cp -p $prod/InstrumentList.json $prod/bkup/InstrumentList.json.$stamp
cp -p $prod/ExchangeList.json $prod/bkup/ExchangeList.json.$stamp

python /home/bridgeplace/scripts/create_instrumentlist.py
python /home/bridgeplace/scripts/create_exchangelist.py


#copy to prod folder
cp -p $dev/InstrumentList.json $prod/
cp -p $dev/ExchangeList.json $prod/