# ATrainer

## Setup

1. In the repo run:

   ```
   dotnet dev-certs https -ep .aspnet/https/aitrainer.pfx -p password -t -v
   dotnet dev-certs https --trust
   ```

2. Open a new terminal

   ```
   cd src/AiTrainer.Core
   npm i
   ```

3. Open a new terminal

   ```
   cd src/aitrainer-test-client
   npm i
   ```

4. Open docker desktop

5. Open a new terminal

   ```
   .\startup.ps1
   ```

   #### You can use flags:

   ##### -webDebug (runs core api in container and you need to manually run the web api)

   ##### -debug (only runs db and identity in container you need to manually run web and core apis)
