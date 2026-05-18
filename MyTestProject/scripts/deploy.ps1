# Deployment script for GitHub free-tier services
# Supports: Render, Railway, Fly.io, and local testing

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('render', 'railway', 'flyio', 'codespaces', 'local')]
    [string]$Service,
    
    [string]$ApiPort = "5000",
    [string]$ClientPort = "3000",
    [string]$Environment = "Development"
)

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Clinical Patient System Deployment" -ForegroundColor Cyan
Write-Host "Free-Tier Service: $Service" -ForegroundColor Yellow
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Function to check if command exists
function Test-Command {
    param([string]$Command)
    $null = Get-Command $Command -ErrorAction SilentlyContinue
    return $?
}

# Function to build application
function Build-Application {
    Write-Host "📦 Building application..." -ForegroundColor Green
    
    try {
        dotnet restore
        if ($LASTEXITCODE -ne 0) { throw "Restore failed" }
        
        dotnet build -c Release
        if ($LASTEXITCODE -ne 0) { throw "Build failed" }
        
        Write-Host "✅ Build successful" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "❌ Build failed: $_" -ForegroundColor Red
        return $false
    }
}

# Function to publish for Linux (for cloud services)
function Publish-ForLinux {
    Write-Host "📦 Publishing for Linux x64..." -ForegroundColor Green
    
    try {
        dotnet publish -c Release -r linux-x64 -o ./dist/api -p:PublishTrimmed=false
        if ($LASTEXITCODE -ne 0) { throw "Publish failed" }
        
        Write-Host "✅ Publish successful to ./dist/api" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "❌ Publish failed: $_" -ForegroundColor Red
        return $false
    }
}

# Service-specific deployment
switch ($Service) {
    "render" {
        Write-Host "🚀 Preparing deployment for Render.com..." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Prerequisites:" -ForegroundColor Cyan
        Write-Host "  ✓ GitHub repository is public"
        Write-Host "  ✓ Render.com account created and linked to GitHub"
        Write-Host "  ✓ PostgreSQL database created in Render"
        Write-Host ""
        
        if (-not (Build-Application)) { exit 1 }
        
        Write-Host ""
        Write-Host "📋 Next steps:" -ForegroundColor Cyan
        Write-Host "  1. Go to https://render.com/dashboard"
        Write-Host "  2. Click 'New' → 'Web Service'"
        Write-Host "  3. Connect your GitHub repository"
        Write-Host "  4. Use these settings:"
        Write-Host "     - Branch: dev"
        Write-Host "     - Build: dotnet publish -c Release -r linux-x64 -o /etc/render/out"
        Write-Host "     - Start: /etc/render/out/ClinicalPatientManagement.Api"
        Write-Host "  5. Add environment variables (see STEP17_DEVOPS_GUIDE.md)"
        Write-Host "  6. Click Deploy"
        Write-Host ""
        Write-Host "📚 See: scripts/deploy-render.md for detailed instructions"
    }
    
    "railway" {
        Write-Host "🚀 Preparing deployment for Railway.app..." -ForegroundColor Yellow
        Write-Host ""
        
        # Check if Railway CLI is installed
        if (-not (Test-Command "railway")) {
            Write-Host "⚠️  Railway CLI not found. Install it:" -ForegroundColor Yellow
            Write-Host "   npm install -g @railway/cli" -ForegroundColor Cyan
            exit 1
        }
        
        if (-not (Build-Application)) { exit 1 }
        
        Write-Host ""
        Write-Host "📋 Deploying to Railway..." -ForegroundColor Cyan
        
        try {
            # Login to Railway
            Write-Host "1️⃣  Logging in to Railway..." -ForegroundColor Green
            railway login
            
            # Create new project
            Write-Host "2️⃣  Creating Railway project..." -ForegroundColor Green
            railway init
            
            # Deploy
            Write-Host "3️⃣  Deploying application..." -ForegroundColor Green
            railway up
            
            Write-Host ""
            Write-Host "✅ Deployment complete!" -ForegroundColor Green
            Write-Host "View your app: railway open" -ForegroundColor Cyan
        }
        catch {
            Write-Host "❌ Railway deployment failed: $_" -ForegroundColor Red
            exit 1
        }
    }
    
    "flyio" {
        Write-Host "🚀 Preparing deployment for Fly.io..." -ForegroundColor Yellow
        Write-Host ""
        
        # Check if Fly CLI is installed
        if (-not (Test-Command "flyctl")) {
            Write-Host "⚠️  Fly CLI not found. Install it:" -ForegroundColor Yellow
            Write-Host "   curl -L https://fly.io/install.sh | sh" -ForegroundColor Cyan
            Write-Host "   Or on Windows: choco install flyctl" -ForegroundColor Cyan
            exit 1
        }
        
        if (-not (Build-Application)) { exit 1 }
        
        Write-Host ""
        Write-Host "📋 Deploying to Fly.io..." -ForegroundColor Cyan
        
        try {
            # Login to Fly
            Write-Host "1️⃣  Logging in to Fly.io..." -ForegroundColor Green
            flyctl auth login
            
            # Create app
            Write-Host "2️⃣  Creating Fly app..." -ForegroundColor Green
            flyctl apps create clinical-patient-system
            
            # Deploy
            Write-Host "3️⃣  Deploying application..." -ForegroundColor Green
            flyctl deploy
            
            Write-Host ""
            Write-Host "✅ Deployment complete!" -ForegroundColor Green
            Write-Host "View your app: flyctl open" -ForegroundColor Cyan
        }
        catch {
            Write-Host "❌ Fly.io deployment failed: $_" -ForegroundColor Red
            exit 1
        }
    }
    
    "codespaces" {
        Write-Host "🚀 Setting up GitHub Codespaces environment..." -ForegroundColor Yellow
        Write-Host ""
        Write-Host "This script runs inside a GitHub Codespace" -ForegroundColor Cyan
        Write-Host ""
        
        if (-not (Build-Application)) { exit 1 }
        
        # Start SQL Server in Docker
        Write-Host "📦 Starting SQL Server container..." -ForegroundColor Green
        docker run -d `
            -e "ACCEPT_EULA=Y" `
            -e "SA_PASSWORD=TestPassword123!" `
            -p 1433:1433 `
            mcr.microsoft.com/mssql/server:2022-latest
        
        Start-Sleep -Seconds 10
        
        # Run migrations
        Write-Host "📝 Running database migrations..." -ForegroundColor Green
        dotnet ef database update `
            --project MyTestProject/Implimentation/patient_management_system/ClinicalPatientManagement/ClinicalPatientManagement.Api
        
        # Run application
        Write-Host "🚀 Starting application..." -ForegroundColor Green
        Write-Host ""
        Write-Host "Access at: https://codespaces-<id>.github.dev" -ForegroundColor Cyan
        Write-Host ""
        
        dotnet run --project MyTestProject/Implimentation/patient_management_system/ClinicalPatientManagement/ClinicalPatientManagement.Api
    }
    
    "local" {
        Write-Host "🚀 Setting up local development environment..." -ForegroundColor Yellow
        Write-Host ""
        
        # Check prerequisites
        if (-not (Test-Command "dotnet")) {
            Write-Host "❌ .NET SDK not installed" -ForegroundColor Red
            exit 1
        }
        
        if (-not (Test-Command "sqlcmd")) {
            Write-Host "⚠️  SQL Server tools not found. Install SQL Server Express or use Docker" -ForegroundColor Yellow
        }
        
        if (-not (Build-Application)) { exit 1 }
        
        # Start SQL Server (if Docker available)
        if (Test-Command "docker") {
            Write-Host ""
            Write-Host "📦 Starting SQL Server in Docker..." -ForegroundColor Green
            docker run -d `
                -e "ACCEPT_EULA=Y" `
                -e "SA_PASSWORD=TestPassword123!" `
                -p 1433:1433 `
                --name sqlserver-local `
                mcr.microsoft.com/mssql/server:2022-latest
            
            Start-Sleep -Seconds 10
            Write-Host "✅ SQL Server running at localhost:1433" -ForegroundColor Green
        }
        else {
            Write-Host "⚠️  Docker not found. Please start SQL Server manually" -ForegroundColor Yellow
        }
        
        # Run migrations
        Write-Host ""
        Write-Host "📝 Running database migrations..." -ForegroundColor Green
        
        try {
            dotnet ef database update `
                --project MyTestProject/Implimentation/patient_management_system/ClinicalPatientManagement/ClinicalPatientManagement.Api
            Write-Host "✅ Migrations complete" -ForegroundColor Green
        }
        catch {
            Write-Host "⚠️  Migration failed. Database may not be ready." -ForegroundColor Yellow
        }
        
        # Run application
        Write-Host ""
        Write-Host "🚀 Starting application..." -ForegroundColor Green
        Write-Host "API: http://localhost:5000" -ForegroundColor Cyan
        Write-Host "Client: http://localhost:3000 (if using separate dev server)" -ForegroundColor Cyan
        Write-Host ""
        
        dotnet run --project MyTestProject/Implimentation/patient_management_system/ClinicalPatientManagement/ClinicalPatientManagement.Api
    }
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Deployment script complete" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
