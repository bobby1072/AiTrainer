
docker compose -f ./../docker-compose.yml -f docker-compose.k6.yml -f ./../docker-compose.coreapi.yml up -d --build

sleep 4


k6 run Tests/UploadFile.js

k6 run Tests/GetOneLayer.js

k6 run Tests/SaveFileCollection.js

k6 run Tests/SyncFileCollectionFaiss.js