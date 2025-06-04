docker compose -f ./../docker-compose.yml -f docker-compose.k6.yml -f ./../docker-compose.coreapi.yml up -d --build

sleep 4


k6 run Tests/UploadFile.js --out influxdb=http://localhost:8086/k6 --http-debug

# k6 run Tests/SaveFileCollection.js --out influxdb=http://localhost:8086/k6 --http-debug

# k6 run Tests/GetOneLayer.js --out influxdb=http://localhost:8086/k6 --http-debug

# k6 run Tests/SyncFileCollectionFaiss.js --out influxdb=http://localhost:8086/k6 --http-debug

# k6 run Tests/SimilaritySearch.js --out influxdb=http://localhost:8086/k6 --http-debug