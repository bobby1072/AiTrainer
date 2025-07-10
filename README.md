# AI Trainer

## Overview

AI Trainer is a comprehensive document processing and AI-powered training platform that enables intelligent document analysis, similarity search, and knowledge extraction. The application combines modern web technologies with advanced AI capabilities to create an interactive training environment for document-based learning and analysis.

### Purpose

The AI Trainer platform is designed to:

- **Process and analyze documents** using advanced chunking algorithms and vector embeddings
- **Provide intelligent similarity search** across document collections using FAISS (Facebook AI Similarity Search)
- **Enable AI-powered interactions** with document content through OpenAI integration
- **Support scalable document management** with efficient storage and retrieval systems

### Key Features

- 🔍 **Advanced Document Processing**: Intelligent chunking and semantic analysis of various document formats
- 🤖 **AI-Powered Similarity Search**: Find relevant content across large document collections
- 🔐 **Secure Authentication**: OAuth/OIDC integration for user management
- 🐳 **Containerized Architecture**: Docker-based deployment for easy scaling and management

## Architecture

### System Components

The AI Trainer platform follows a microservices architecture with the following key components:

#### 1. **Backend APIs**

- **AiTrainer.Core** (`src/AiTrainer.Core/`)
  - Node.js/Express API for AI and document processing
  - LangChain integration for document chunking and embeddings
  - FAISS vector store for similarity search
  - OpenAI integration for AI-powered features
- **AiTrainer.Web.Api** (`src/AiTrainer.Web/`)
  - .NET 9.0 Web API for business logic and data management
  - JWT authentication and authorization
  - Entity Framework for data persistence

#### 2. **Data Layer**

- **PostgreSQL Database**: Primary data storage for application data

#### 3. **Infrastructure**

- **Docker Compose**: Container orchestration for development and deployment
- **OIDC Server**: Authentication and identity management
- **K6**: Load testing and performance monitoring

### Data Flow

1. **Document Upload**: Users upload documents
2. **Processing**: Documents are sent to the Core API for chunking and vectorization
3. **Storage**: Processed chunks are stored in FAISS for similarity search
4. **Query Processing**: User queries are vectorized and matched against the document store
5. **AI Enhancement**: OpenAI services provide intelligent responses and insights
6. **Real-time Updates**: SignalR ensures all connected users receive live updates

### Technology Stack

| Layer            | Technology                            |
| ---------------- | ------------------------------------- |
| Backend          | .NET 9.0, Node.js/Express, TypeScript |
| AI/ML            | LangChain, OpenAI API, FAISS          |
| Database         | PostgreSQL                            |
| Authentication   | OIDC/OAuth                            |
| Containerization | Docker, Docker Compose                |
| Testing          | Jest, XUNIT, K6 Load Testing          |

## Prerequisites

### Required Software

- **Docker Desktop** - For container orchestration and database services
- **Node.js** (v18 or higher) - For running the Core API and frontend
- **NPM** - Package manager for Node.js dependencies
- **.NET 9.0 SDK** - For building and running the Web API
- **Development IDE** - Visual Studio, JetBrains Rider, or VS Code recommended

### Optional Tools

- **PostgreSQL Client** - For direct database access and debugging
- **Postman** - For API testing and development
- **Git** - For version control and collaboration

## Quick Start

### Development Setup

1. **Generate HTTPS certificates for development:**

   ```powershell
   dotnet dev-certs https -ep .aspnet/https/aitrainer.pfx -p password -t -v
   dotnet dev-certs https --trust
   ```

2. **Install Core API dependencies:**

   ```powershell
   cd src/AiTrainer.Core
   npm install
   ```

3. **Install Frontend dependencies:**

   ```powershell
   cd src/aitrainer-test-client
   npm install
   ```

4. **Start services:**
   ```powershell
   # Ensure Docker Desktop is running
   .\startup.ps1
   ```

### Startup Script Options

The `startup.ps1` script supports different development modes:

- **Full Container Mode** (default): `.\startup.ps1`

  - Runs all services in containers
  - Best for testing the complete system

- **Web Debug Mode**: `.\startup.ps1 -webDebug`

  - Runs Core API in container
  - Allows debugging the .NET Web API locally

- **Full Debug Mode**: `.\startup.ps1 -debug`
  - Only runs database and identity services in containers
  - Allows debugging both Web and Core APIs locally

### Development Workflow

For active development on the Core API:

```powershell
cd src/AiTrainer.Core
npm run start:dev
```

This enables hot-reload functionality without rebuilding containers.

## Docker Services

The platform includes several containerized services:

| Service     | Port      | Purpose                             |
| ----------- | --------- | ----------------------------------- |
| PostgreSQL  | 5560      | Primary database                    |
| OIDC Server | 44363     | Authentication provider             |
| Core API    | 3000      | Document processing and AI services |
| Web API     | 5000/5001 | Business logic and data management  |

## Development Guidelines

### Project Structure

```
src/
├── aitrainer-test-client/     # React test frontend application
├── AiTrainer.Core/            # Node.js API for AI/ML operations
└── AiTrainer.Web/             # .NET Web API and services
    ├── AiTrainer.Web.Api/           # Main Web API
    ├── AiTrainer.Web.Api.SignalR/  # Real-time communication
    ├── AiTrainer.Web.Domain.*/     # Business logic layers
    └── AiTrainer.Web.*.Tests/      # Test projects
```

### Testing

- **Unit Tests**: Run `npm test` in Core API or `dotnet test` for Web API
- **Load Testing**: Use K6 scripts in the `K6/` directory

## API Documentation

### Core API Endpoints

- **Health Check**: `GET /api/health`
- **Document Processing**: `POST /api/chunking/process`
- **FAISS Operations**: `POST /api/faiss/*`
- **Similarity Search**: `POST /api/faiss/similarity-search`
- **OpenAI Integration**: `POST /api/openai/*`

### Web API Endpoints

- **Authentication**: `POST /api/auth/*`
- **User Management**: `GET|POST /api/users/*`
- **Document Management**: `GET|POST|PUT|DELETE /api/documents/*`
- **Training Sessions**: `GET|POST /api/training/*`

## Troubleshooting

### Common Issues

1. **Certificate Issues**: Re-run the certificate generation commands
2. **Port Conflicts**: Check that required ports (5560, 44363, 3000, 5000) are available
3. **Container Issues**: Restart Docker Desktop and run `docker-compose down && docker-compose up`
4. **Node Modules**: Clear npm cache with `npm cache clean --force` and reinstall

### Logs and Debugging

- **Container Logs**: `docker-compose logs [service-name]`
- **Core API Logs**: Check console output when running in development mode
- **Web API Logs**: Available in Visual Studio output or console when running locally

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes following the established patterns
4. Add tests for new functionality
5. Submit a pull request with a clear description

## License

This project is licensed under a custom non-commercial license. Commercial use is prohibited without explicit written permission from the copyright holder. See the [LICENSE](LICENSE) file for full terms.

---

## Extra Development Notes

#### - in the AiTrainer.Core api you can use command 'npm run start:dev' to not have to keep rebuilding and compiling the project
