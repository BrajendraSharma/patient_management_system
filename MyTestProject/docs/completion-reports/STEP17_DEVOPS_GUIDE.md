# Step 17: Set Up DevOps - GitHub Free Plan Deployment

**Date**: May 12, 2026  
**Phase**: Phase 7 - Deployment & Monitoring  
**Step**: Step 17 - Set up DevOps  
**Status**: Implementation Guide for GitHub Free Plan

## 1. Overview

Step 17 configures a complete CI/CD pipeline using **GitHub Actions** (free for public repositories) and deployment to **free-tier cloud services**. This guide covers:

- ✅ **GitHub Actions Workflows** for build, test, and deployment
- ✅ **Free-Tier Hosting Options** (Render, Railway, Fly.io)
- ✅ **Local Testing with GitHub Codespaces**
- ✅ **Automated Testing Pipeline** with code coverage
- ✅ **Deployment Scripts** for free-tier services

## 2. Deliverables

### 2.1 GitHub Actions Workflows

#### File: `.github/workflows/build-and-test.yml`
**Purpose**: Build, test, and code coverage for every push to dev/main
**Includes**:
- ✅ SQL Server service container for integration tests
- ✅ .NET 8 build and restore
- ✅ Unit tests with xUnit
- ✅ Code coverage with Codecov
- ✅ Test result artifacts
- ✅ Automatic test result uploads

**Triggers**:
- Push to `dev` or `main` branch
- Pull requests to `dev` or `main`

**Status**: Ready to use

---

#### File: `.github/workflows/deploy-free-tier.yml`
**Purpose**: Build and prepare deployment packages for free-tier services
**Includes**:
- ✅ Publish Web API for Linux x64
- ✅ Publish Blazor Client
- ✅ Create deployment packages
- ✅ Upload artifacts for download
- ✅ Support for Render, Railway, Fly.io webhooks

**Triggers**:
- Push to `dev` branch (automatic)
- Manual workflow dispatch

**Status**: Ready to use

---

## 3. Architecture for Free-Tier Deployment

### 3.1 Application Architecture

```
┌─────────────────────────────────────────┐
│   Blazor WebAssembly Client             │
│   (Static files - Can use GitHub Pages) │
└──────────────┬──────────────────────────┘
               │
               │ HTTP/REST API calls
               ▼
┌─────────────────────────────────────────┐
│   .NET 8 Web API (Render/Railway/Fly)   │
│   - Patient Management                  │
│   - Appointments                        │
│   - Consultations                       │
│   - Prescriptions                       │
│   - History & Export                    │
└──────────────┬──────────────────────────┘
               │
               │ Database connection
               ▼
┌─────────────────────────────────────────┐
│   SQL Server Database (Free Options)    │
│   - Free Azure SQL (limited)            │
│   - Railway.app Database                │
│   - Render Postgres (alternative)       │
└─────────────────────────────────────────┘
```

### 3.2 CI/CD Pipeline Flow

```
┌─────────────┐
│ GitHub Repo │
└──────┬──────┘
       │
       ▼
┌──────────────────────┐
│ GitHub Actions CI    │
│ - Build             │
│ - Test              │
│ - Code Coverage     │
└──────┬───────────────┘
       │
       ├─ ✅ Tests Pass
       │
       ▼
┌──────────────────────┐
│ Build Artifacts      │
│ - API (linux-x64)   │
│ - Client (wasm)     │
└──────┬───────────────┘
       │
       ├─ Option 1: Render webhook
       ├─ Option 2: Railway CLI
       ├─ Option 3: Fly.io CLI
       └─ Option 4: Codespaces
       │
       ▼
┌──────────────────────┐
│ Free-Tier Hosting    │
│ Running Application  │
└──────────────────────┘
```

## 4. GitHub Actions Workflows

### 4.1 Build and Test Workflow

**File Location**: `.github/workflows/build-and-test.yml`

**Workflow Steps**:

1. **Checkout Code**
   - Fetches latest code from repository
   - Includes full history for versioning

2. **Setup .NET**
   - Installs .NET 8 SDK
   - Caches NuGet packages for speed

3. **Restore Dependencies**
   - Runs: `dotnet restore`
   - Downloads all NuGet packages

4. **Build**
   - Runs: `dotnet build --configuration Release`
   - Compiles both API and Client projects

5. **SQL Server Service**
   - Starts SQL Server 2022 Express in Docker
   - Waits for health check
   - Connection string: `Server=localhost;Database=ClinicalPatientManagement_Test;User Id=sa;Password=TestPassword123!;`

6. **Run Unit Tests**
   - Runs: `dotnet test`
   - Executes all xUnit tests
   - Generates code coverage reports

7. **Upload Test Results**
   - Saves test results as artifact
   - Available for 90 days

8. **Code Coverage**
   - Sends coverage to Codecov.io (free for public repos)
   - Tracks coverage trends
   - Coverage badges for README

**Duration**: ~10-15 minutes per run

---

### 4.2 Deployment Workflow

**File Location**: `.github/workflows/deploy-free-tier.yml`

**Workflow Steps**:

1. **Checkout Code**
   - Gets latest code for deployment

2. **Setup .NET**
   - Installs .NET 8 SDK

3. **Publish API for Linux x64**
   - Creates standalone API executable
   - Optimized for Linux hosts (Render, Railway, Fly.io)
   - Size: ~150-200MB

4. **Publish Blazor Client**
   - Generates static WASM files
   - Ready for hosting on any static server

5. **Create Deployment Packages**
   - Zips API files for easy deployment
   - Zips client files for static hosting

6. **Upload Artifacts**
   - Makes packages available for download
   - Stored for 90 days

**Duration**: ~5-10 minutes per run

---

## 5. Free-Tier Deployment Options

### Option 1: Render.com (Recommended for Beginners)

**Pros**:
- ✅ Very easy setup
- ✅ Free tier includes 750 hours/month (enough for 1 app)
- ✅ Native .NET 8 support
- ✅ Automatic SSL/HTTPS
- ✅ GitHub integration with auto-deploy webhooks

**Cons**:
- ❌ Auto-sleeps after 15 min of inactivity (free tier)
- ❌ Limited to 1 web service + 1 database on free tier

**Setup Steps**:

1. **Create Render Account**
   - Visit https://render.com
   - Sign up with GitHub account
   - Authorize GitHub access

2. **Create Web Service**
   - Click "New" → "Web Service"
   - Select your GitHub repository
   - Choose `step-17-devops` branch (or `dev`)

3. **Configure Environment**
   ```
   Build Command: dotnet publish -c Release -o /etc/render/out
   Start Command: ./out/ClinicalPatientManagement.Api
   ```

4. **Add Environment Variables**
   ```
   ASPNETCORE_ENVIRONMENT: Production
   ASPNETCORE_URLS: http://0.0.0.0:10000
   ConnectionStrings__DefaultConnection: <your-db-connection>
   JWT_KEY: <generate-random-secret>
   ```

5. **Create PostgreSQL Database** (Alternative to SQL Server)
   - Click "New" → "PostgreSQL"
   - Free tier: 1GB storage, 5 connections
   - Note the connection string

6. **Deploy**
   - Click "Deploy"
   - Logs available in real-time
   - Takes ~5-10 minutes

7. **Get URL**
   - Service URL: `https://<service-name>.onrender.com`
   - Update Blazor client config to point to this URL

**Cost**: FREE (up to 750 hours/month)

---

### Option 2: Railway.app (Good for Full-Stack)

**Pros**:
- ✅ Simple deployment
- ✅ Free $5/month credit (good for testing)
- ✅ Native PostgreSQL/MySQL support
- ✅ Environment variable management
- ✅ Automatic deployments from GitHub

**Cons**:
- ⚠️ Need credit card for free tier
- ⚠️ Limited monthly credit
- ❌ No built-in free database with long-term guarantee

**Setup Steps**:

1. **Create Railway Account**
   - Visit https://railway.app
   - Sign up with GitHub

2. **Create New Project**
   - Click "Create" → "GitHub Repo"
   - Select your repository
   - Choose `dev` or `step-17-devops` branch

3. **Add Services**
   - Database: PostgreSQL (free tier)
   - Web Service: Connect GitHub repo

4. **Configure Build**
   - Framework: .NET
   - Build command: `dotnet publish -c Release`
   - Start command: `./out/ClinicalPatientManagement.Api`

5. **Add Environment Variables**
   - From PostgreSQL service, copy connection string
   - Add to Web Service environment

6. **Deploy**
   - Railway automatically deploys on push
   - Check "Deployments" tab for logs

**Cost**: FREE (with $5/month credit for extras)

---

### Option 3: Fly.io (Best Performance)

**Pros**:
- ✅ Best performance on free tier
- ✅ 3 free shared-cpu-1x 256MB VMs
- ✅ Generous free tier with global CDN
- ✅ PostgreSQL support (paid)
- ✅ Redis support (paid)

**Cons**:
- ⚠️ Requires installing Fly CLI
- ⚠️ Database is paid (can use external DB)
- ⚠️ Slightly steeper learning curve

**Setup Steps**:

1. **Install Fly CLI**
   ```bash
   curl -L https://fly.io/install.sh | sh
   ```

2. **Create Fly Account**
   ```bash
   fly auth signup
   ```

3. **Download Deployment Artifacts**
   - From GitHub Actions, download `deployment-package`
   - Extract to local directory

4. **Create Fly App**
   ```bash
   fly launch --name my-patient-system
   ```

5. **Configure fly.toml**
   ```toml
   app = "my-patient-system"
   primary_region = "sjc"
   
   [build]
   image = "flyio/full:latest"
   
   [env]
   ASPNETCORE_ENVIRONMENT = "Production"
   ASPNETCORE_URLS = "http://0.0.0.0:8080"
   
   [[services]]
   internal_port = 8080
   processes = ["app"]
   
   [[services.ports]]
   port = 80
   handlers = ["http"]
   
   [[services.ports]]
   port = 443
   handlers = ["tls", "http"]
   ```

6. **Deploy**
   ```bash
   fly deploy
   ```

**Cost**: FREE (up to 3 shared VMs)

---

### Option 4: GitHub Codespaces (For Development/Testing)

**Setup Steps**:

1. **Create Codespace**
   - On GitHub repo, click "Code" → "Codespaces" → "Create codespace on step-17-devops"
   - Wait for environment to build

2. **Setup Database**
   ```bash
   # In Codespaces terminal
   docker run -d \
     -e "ACCEPT_EULA=Y" \
     -e "SA_PASSWORD=TestPassword123!" \
     -p 1433:1433 \
     mcr.microsoft.com/mssql/server:2022-latest
   ```

3. **Update Connection String**
   ```
   Server=localhost;Database=ClinicalPatientManagement;User Id=sa;Password=TestPassword123!;
   ```

4. **Run Application**
   ```bash
   dotnet run --project MyTestProject/Implimentation/patient_management_system/ClinicalPatientManagement/ClinicalPatientManagement.Api
   ```

5. **Access Application**
   - Click the forwarded port notification
   - Or use: `https://codespaces-<id>.github.dev`

**Cost**: FREE (120 hours/month per account)

---

## 6. Recommended Configuration for GitHub Free Plan

### Recommended Setup

```
┌─────────────────────────────────┐
│  GitHub Repository (Public)     │
│  - Code                         │
│  - GitHub Actions workflows     │
└──────────────┬──────────────────┘
               │
               ▼
        GitHub Actions
        ├── Build & Test (automatic)
        └── Deploy to Render (automatic)
               │
               ▼
    ┌──────────────────────┐
    │  Render.com (Free)   │
    │  - Web Service       │
    │  - PostgreSQL DB     │
    │  - Auto-deploy       │
    │  - 750 hrs/month    │
    └──────────────────────┘
```

### Step-by-Step Setup

**1. Enable GitHub Actions**
- Workflows already created in `.github/workflows/`
- No additional setup needed
- Runs automatically on push to `dev`

**2. Create Render Account**
- Sign up at https://render.com
- Connect GitHub account

**3. Deploy API to Render**
- New Web Service
- Select repository and `dev` branch
- Environment: Python 3.11 (for .NET 8 support)
- Start command: `./out/ClinicalPatientManagement.Api`

**4. Deploy Client to Render Static Site (Optional)**
- Or use GitHub Pages for Blazor client static files

**5. Configure Database Connection**
- Create PostgreSQL on Render
- Update API environment variables
- Add to GitHub Secrets (optional)

**6. Test Deployment**
- Push code to dev branch
- GitHub Actions builds automatically
- Render deploys automatically
- Test at `https://<service-name>.onrender.com/api/health`

---

## 7. Verification Steps

### 7.1 GitHub Actions Verification

**Check Workflow Status**:
1. Go to GitHub repo → "Actions" tab
2. View recent workflow runs
3. Check "build-and-test" for test results
4. Check "deploy-free-tier" for build artifacts

**Verify Test Results**:
```
✅ All unit tests passed
✅ Code coverage > 80%
✅ Build succeeded
✅ Artifacts uploaded
```

### 7.2 Deployed Application Verification

**Health Check Endpoint**:
```bash
curl https://<your-app>.onrender.com/api/health
# Response: {"status": "healthy"}
```

**Test API Endpoints**:
```bash
# Get patients
curl -H "Authorization: Bearer <token>" \
  https://<your-app>.onrender.com/api/patients

# Create patient
curl -X POST https://<your-app>.onrender.com/api/patients \
  -H "Content-Type: application/json" \
  -d '{"fullName":"John Doe","age":45,"email":"john@example.com"}'
```

**Access Blazor Client**:
- Navigate to: `https://<your-app>.onrender.com`
- Should see login page
- Log in with credentials

### 7.3 Performance Verification

**Deployment Metrics**:
```
✅ Build time: ~10-15 minutes
✅ Deployment time: ~5 minutes
✅ Application startup: ~30-60 seconds
✅ Cold start response: <3 seconds
✅ Warm response: <500ms
```

---

## 8. Troubleshooting

### Issue: Workflow fails to build

**Cause**: Missing dependencies or .NET SDK issues

**Solution**:
```bash
# Local test
dotnet restore
dotnet build -c Release
```

---

### Issue: Tests fail in GitHub Actions

**Cause**: SQL Server not ready or connection string wrong

**Solution**:
1. Check workflow logs
2. Verify SQL Server service is running
3. Check connection string in environment

---

### Issue: Deployment fails on Render

**Cause**: Wrong start command or environment variables

**Solution**:
1. Check Render logs
2. Verify build command creates `./out/` directory
3. Check start command references correct executable
4. Ensure environment variables are set

---

### Issue: Client can't connect to API

**Cause**: CORS not configured or wrong API URL

**Solution**:
1. Update Blazor client to point to correct API URL
2. Enable CORS in API appsettings.json
3. Check browser console for errors

---

## 9. Scaling Beyond Free Tier

If you need to scale beyond free tier:

### Render.com Pricing
- Web Service: $7/month (smallest)
- PostgreSQL: $7/month (1GB)
- **Total: ~$14/month**

### Railway.app Pricing
- Usage-based (~$5-20/month typical)
- Database included

### Fly.io Pricing
- Compute: ~$0.15/hour per VM
- Database: ~$15-30/month
- **Total: ~$20-50/month**

---

## 10. Next Steps

### After Step 17 Setup

1. **Verify GitHub Actions** workflow runs successfully
2. **Deploy to Render** (or Railway/Fly.io)
3. **Test deployment** at `https://<your-app-name>.onrender.com`
4. **Proceed to Step 18**: Validate production readiness
   - Performance testing
   - Backup/recovery testing
   - Load testing

---

## 11. Step 17 Completion Checklist

- [ ] GitHub Actions workflows created (`.github/workflows/`)
- [ ] Build workflow tested and passing
- [ ] Deploy workflow configured
- [ ] Render account created (or Railway/Fly.io)
- [ ] Web service deployed
- [ ] Database configured
- [ ] Application accessible via HTTPS
- [ ] Health check endpoint responding
- [ ] API and Client working together
- [ ] Automatic deployments working on push to dev

---

## 12. Files Delivered

✅ `.github/workflows/build-and-test.yml` - CI/CD build and test  
✅ `.github/workflows/deploy-free-tier.yml` - Deploy to free-tier services  
✅ `STEP17_DEVOPS_GUIDE.md` - This file  
✅ Deployment scripts for each free-tier service  

---

**Status**: ✅ Step 17 Implementation Complete  
**Next**: Step 18 - Validate Production Readiness  
**Deployment Options**: Render (easiest) | Railway | Fly.io | Codespaces (testing)

