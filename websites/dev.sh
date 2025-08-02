#!/bin/bash

# AlienTranslate Local Development Script
# Usage: ./dev.sh [start|stop|restart|logs|status]

set -e

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="$PROJECT_DIR/docker-compose.dev.yml"

# Colors for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Logging function
log() {
    echo -e "${GREEN}[$(date +'%Y-%m-%d %H:%M:%S')]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1" >&2
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

# Check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        error "Docker is not running. Please start Docker first."
        exit 1
    fi
}

# Create necessary directories
create_directories() {
    log "Creating necessary directories..."
    mkdir -p translation-server/data translation-server/logs vue-alientranslate/data
    chmod 755 translation-server/data translation-server/logs vue-alientranslate/data
}

# Create environment files if they don't exist
create_env_files() {
    log "Checking environment files..."
    
    if [[ ! -f "translation-server/.env" ]]; then
        cat > translation-server/.env << 'EOF'
NODE_ENV=development
OPENAI_API_KEY=your-openai-api-key-here
ADMIN_PASSWORD=admin123
EOF
        warning "Created translation-server/.env - Please update with your actual API keys"
    fi
    
    if [[ ! -f "vue-alientranslate/.env" ]]; then
        cat > vue-alientranslate/.env << 'EOF'
NODE_ENV=development
BASE_URL=http://localhost:5173
JWT_SECRET=dev-secret-key-change-in-production
EMAIL_USER=your-email@gmail.com
EMAIL_PASS=your-app-password
EOF
        warning "Created vue-alientranslate/.env - Please update with your actual values"
    fi
}

# Start function
start() {
    log "Starting development services..."
    docker-compose -f "$COMPOSE_FILE" up -d
    log "Services started successfully!"
    
    # Wait a moment for services to start
    sleep 5
    
    # Show status
    status
    
    # Show access URLs
    echo ""
    echo "🌐 Access your applications at:"
    echo "   Vue.js App: http://localhost:5173"
    echo "   Vue.js API: http://localhost:3001"
    echo "   Translation API: http://localhost:3333"
    echo ""
    echo "📝 View logs with: ./dev.sh logs"
}

# Stop function
stop() {
    log "Stopping development services..."
    docker-compose -f "$COMPOSE_FILE" down
    log "Services stopped successfully!"
}

# Restart function
restart() {
    log "Restarting development services..."
    docker-compose -f "$COMPOSE_FILE" restart
    log "Services restarted successfully!"
    
    # Show status
    status
}

# Status function
status() {
    log "Checking service status..."
    echo ""
    docker-compose -f "$COMPOSE_FILE" ps
    echo ""
    
    # Check health endpoints
    log "Checking health endpoints..."
    
    # Check if services are responding
    if curl -s -f http://localhost:3001/api/health > /dev/null 2>&1; then
        echo -e "${GREEN}✓ Vue.js API is healthy${NC}"
    else
        echo -e "${RED}✗ Vue.js API is not responding${NC}"
    fi
    
    if curl -s -f http://localhost:3333/health > /dev/null 2>&1; then
        echo -e "${GREEN}✓ Translation API is healthy${NC}"
    else
        echo -e "${RED}✗ Translation API is not responding${NC}"
    fi
}

# Logs function
logs() {
    log "Showing logs (press Ctrl+C to exit)..."
    docker-compose -f "$COMPOSE_FILE" logs -f
}

# Build function
build() {
    log "Building development images..."
    docker-compose -f "$COMPOSE_FILE" build --no-cache
    log "Build completed successfully!"
}

# Main script logic
main() {
    cd "$PROJECT_DIR"
    
    # Check Docker
    check_docker
    
    case "${1:-help}" in
        "start")
            create_directories
            create_env_files
            start
            ;;
        "stop")
            stop
            ;;
        "restart")
            restart
            ;;
        "status")
            status
            ;;
        "logs")
            logs
            ;;
        "build")
            build
            ;;
        "help"|*)
            echo "AlienTranslate Local Development Script"
            echo ""
            echo "Usage: $0 [command]"
            echo ""
            echo "Commands:"
            echo "  start     - Start development services"
            echo "  stop      - Stop development services"
            echo "  restart   - Restart development services"
            echo "  status    - Show service status and health"
            echo "  logs      - Show service logs"
            echo "  build     - Rebuild development images"
            echo "  help      - Show this help message"
            echo ""
            echo "Examples:"
            echo "  $0 start      # Start development environment"
            echo "  $0 logs       # View logs"
            echo "  $0 status     # Check status"
            ;;
    esac
}

# Run main function
main "$@"