# nginx
sudo nano /etc/nginx/sites-available/apiwebscraper
sudo nginx -t 
sudo systemctl reload nginx

# dotnet /root/webserver/apiWebScraper
sudo nano /etc/systemd/system/apiwebscraper.service
sudo systemctl status apiwebscraper.service
journalctl -u apiwebscraper.service -f

# certbot
sudo certbot --nginx -d webscrapper.cryptoscout.ai
