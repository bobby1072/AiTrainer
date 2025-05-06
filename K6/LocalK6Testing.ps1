
docker compose -f ./../docker-compose.yml -f docker-compose.k6.yml -f ./../docker-compose.coreapi.yml up -d --build


k6 run Tests/UploadFile.js