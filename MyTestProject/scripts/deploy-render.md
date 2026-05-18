# Render.com Deployment Script
# Prerequisites: GitHub account, Render account connected to GitHub

## Step 1: Create Render Web Service

1. Go to https://render.com/dashboard
2. Click "New" → "Web Service"
3. Select your GitHub repository: `patient_management_system`
4. Select branch: `dev` or `step-17-devops`
5. Fill in configuration:
   - **Name**: `clinical-patient-system`
   - **Environment**: `Docker` or `Other`
   - **Build Command**: 
     ```
     dotnet publish -c Release -r linux-x64 -o /etc/render/out
     ```
   - **Start Command**:
     ```
     /etc/render/out/ClinicalPatientManagement.Api
     ```
   - **Instance Type**: Free
   - **Auto-deploy**: Yes

## Step 2: Add Environment Variables

After creating service, go to "Environment":

```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
ConnectionStrings__DefaultConnection=Server=<postgres-host>;Database=patient_mgmt;User ID=<user>;Password=<password>;SSL Mode=Require;
JWT_KEY=<generate-random-32-char-key>
ALLOWED_ORIGINS=https://<your-app>.onrender.com
```

## Step 3: Create PostgreSQL Database

1. Click "New" → "PostgreSQL"
2. Fill in:
   - **Name**: `clinical-patient-db`
   - **Database**: `clinical_patient_system`
   - **User**: `postgres`
3. Click "Create Database"
4. Copy connection string to environment variable above

## Step 4: Configure Database Connection String

From PostgreSQL service page, copy:
```
postgresql://user:password@host:5432/database
```

Convert to SQL Server format:
```
Server=host;Database=database;User ID=user;Password=password;SSL Mode=Require;
```

## Step 5: Update Application Code (if needed)

If using PostgreSQL instead of SQL Server:
1. Update DbContext to use PostgreSQL provider
2. Or keep SQL Server and use Azure SQL free tier

## Step 6: Deploy

1. Click "Deploy"
2. View logs in "Logs" tab
3. Wait for "Service is live" message
4. Access at: `https://<service-name>.onrender.com`

## Step 7: Verify Deployment

```bash
# Test health endpoint
curl https://clinical-patient-system.onrender.com/api/health

# Test API
curl -X GET https://clinical-patient-system.onrender.com/api/patients

# Test Blazor client
curl https://clinical-patient-system.onrender.com
```

## Step 8: Run Database Migrations

After first deployment:

```bash
# In Render dashboard, go to "Shell" tab
dotnet ef database update --project ClinicalPatientManagement.Api
```

Or run migration on application startup by adding to appsettings.json:
```json
{
  "Database": {
    "AutoMigrate": true
  }
}
```

## Ongoing Operations

**View Logs**:
- Dashboard → Your service → "Logs" tab
- Real-time streaming of application logs

**Restart Service**:
- Dashboard → Your service → "Restart"

**Update Environment Variables**:
- Dashboard → Your service → "Environment"
- Changes take effect on next deploy

**Monitor Health**:
- Dashboard → Your service
- Check "Active instances", "CPU", "Memory"

**Auto-deploy from GitHub**:
- Already enabled if you selected "Auto-deploy"
- Any push to `dev` branch triggers deployment

---

## Cost

✅ **FREE** (750 hours/month per service + database)
- 1 Web Service: Free
- 1 PostgreSQL Database: Free
- **Limitation**: Auto-suspends after 15 min of no traffic (free tier)
  - Wakes up when traffic returns (~30 sec startup)

---

## Upgrade to Paid (if needed)

If you need to remove auto-suspend and get better performance:
- **Paid Web Service**: $7/month (upgraded)
- **Paid Database**: $7/month (dedicated instance)
