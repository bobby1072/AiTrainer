# AI Trainer

## Overview

AI Trainer is a comprehensive document processing and AI-powered training platform that enables intelligent document analysis, similarity search, and knowledge extraction. The application combines modern web technologies with advanced AI capabilities to create an interactive training environment for document-based learning and analysis.

### Purpose

The AI Trainer platform is designed to:
- **Process and analyze documents** using advanced chunking algorithms and vector embeddings
- **Provide intelligent similarity search** across document collections using FAISS (Facebook AI Similarity Search)
- **Enable AI-powered interactions** with document content through OpenAI integration
- **Offer real-time collaboration** features through SignalR for multi-user training sessions
- **Support scalable document management** with efficient storage and retrieval systems

### Key Features

- 🔍 **Advanced Document Processing**: Intelligent chunking and semantic analysis of various document formats
- 🤖 **AI-Powered Similarity Search**: Find relevant content across large document collections
- 🌐 **Modern Web Interface**: React-based client application with Material-UI components
- 🔐 **Secure Authentication**: OAuth/OIDC integration for user management
- 📊 **Real-time Analytics**: Performance monitoring with K6 load testing and Grafana dashboards
- 🐳 **Containerized Architecture**: Docker-based deployment for easy scaling and management

## Architecture

### System Components

The AI Trainer platform follows a microservices architecture with the following key components:

#### 1. **Frontend Layer**
- **AI Trainer Test Client** (`src/aitrainer-test-client/`)
  - React 18 with TypeScript
  - Material-UI for modern, responsive interface
  - Real-time updates via SignalR
  - Form validation with React Hook Form and Zod

#### 2. **Backend APIs**
- **AiTrainer.Core** (`src/AiTrainer.Core/`)
  - Node.js/Express API for AI and document processing
  - LangChain integration for document chunking and embeddings
  - FAISS vector store for similarity search
  - OpenAI integration for AI-powered features
  
- **AiTrainer.Web.Api** (`src/AiTrainer.Web/`)
  - .NET 9.0 Web API for business logic and data management
  - SignalR hub for real-time communication
  - JWT authentication and authorization
  - Entity Framework for data persistence

#### 3. **Data Layer**
- **PostgreSQL Database**: Primary data storage for application data
- **FAISS Vector Store**: High-performance similarity search index
- **File Storage**: Document and media file management

#### 4. **Infrastructure**
- **Docker Compose**: Container orchestration for development and deployment
- **OIDC Server**: Authentication and identity management
- **Grafana**: Monitoring and analytics dashboards
- **K6**: Load testing and performance monitoring

### Data Flow

1. **Document Upload**: Users upload documents through the React frontend
2. **Processing**: Documents are sent to the Core API for chunking and vectorization
3. **Storage**: Processed chunks are stored in FAISS for similarity search
4. **Query Processing**: User queries are vectorized and matched against the document store
5. **AI Enhancement**: OpenAI services provide intelligent responses and insights
6. **Real-time Updates**: SignalR ensures all connected users receive live updates

### Technology Stack

| Layer | Technology |
|-------|------------|
| Frontend | React 18, TypeScript, Material-UI, SignalR Client |
| Backend | .NET 9.0, Node.js/Express, TypeScript |
| AI/ML | LangChain, OpenAI API, FAISS |
| Database | PostgreSQL |
| Authentication | OIDC/OAuth |
| Containerization | Docker, Docker Compose |
| Testing | Jest, K6 Load Testing |
| Monitoring | Grafana |

## Prerequisites

#### - Docker desktop

#### - NodeJS

#### - NPM

#### - .NET 9.0

#### - Some .net ide (such as Visual Studio or Jestbrains Rider)

## Prerequisites

### Required Software
- **Docker Desktop** - For container orchestration and database services
- **Node.js** (v18 or higher) - For running the Core API and frontend
- **NPM** - Package manager for Node.js dependencies
- **.NET 9.0 SDK** - For building and running the Web API
- **Development IDE** - Visual Studio, JetBrains Rider, or VS Code recommended

### Optional Tools
- **PostgreSQL Client** - For direct database access and debugging
- **Postman/Insomnia** - For API testing and development
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

4. **Start Docker services:**
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

## Environment Configuration

### Environment Variables

The application uses the following key environment variables:

- `OPENAI_API_KEY` - Your OpenAI API key for AI features
- `POSTGRES_CONNECTION_STRING` - Database connection details
- `JWT_SECRET` - Secret key for JWT token signing
- `OIDC_AUTHORITY` - OIDC server URL for authentication

### Docker Services

The platform includes several containerized services:

| Service | Port | Purpose |
|---------|------|---------|
| PostgreSQL | 5560 | Primary database |
| OIDC Server | 44363 | Authentication provider |
| Core API | 3000 | Document processing and AI services |
| Web API | 5000/5001 | Business logic and data management |
| Frontend | 3001 | React application |

## Development Guidelines

### Project Structure

```
src/
├── aitrainer-test-client/     # React frontend application
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
- **Integration Tests**: Available in respective test projects

### Monitoring

Access Grafana dashboards at `http://localhost:3001/grafana` (when configured) to monitor:
- API performance metrics
- Database query performance
- System resource usage
- Load testing results

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

This project is licensed under the ISC License.

---

## Extra Development Notes

#### - in the AiTrainer.Core api you can use command 'npm run start:dev' to not have to keep rebuilding and compiling the project
