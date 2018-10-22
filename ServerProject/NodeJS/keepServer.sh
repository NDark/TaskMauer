export NVM_DIR=~/.nvm
source ~/.nvm/nvm.sh
nvm install 9.4.0


while [ 1 ]
do

pgrep -f "bash bashServer.sh" > /dev/null
result=$?
echo "exit code: ${result}"

if [ "${result}" -eq "0" ] ;
then
    echo "Running"
    date
    sleep 5
else
    echo "Stopped, Restart node server"
    nohup bash bashServer.sh &
    sleep 5
fi
done

