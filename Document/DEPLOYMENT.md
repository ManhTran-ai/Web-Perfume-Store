# GuhaStore Deployment Guide

This guide provides instructions for deploying GuhaStore to production environments.

## Prerequisites

- Docker and Docker Compose
- Domain name with SSL certificate
- SMTP email service (Gmail, SendGrid, etc.)
- Payment gateway accounts (VNPay, MoMo)
- MySQL database (or use Docker)

## Quick Start with Docker Compose

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/guhastore.git
   cd guhastore
   ```

2. **Configure environment variables**
   ```bash
   cp GuhaStore.Web/appsettings.Production.json.example GuhaStore.Web/appsettings.Production.json
   # Edit the file with your production settings
   ```

3. **Update payment and email credentials**
   Edit `GuhaStore.Web/appsettings.Production.json` with:
   - Database connection string
   - SMTP settings
   - VNPay/MoMo credentials

4. **Build and run**
   ```bash
   docker-compose up -d
   ```

5. **Initialize database** (if not using Docker)
   ```bash
   mysql -u root -p dbperfume < dbperfume.sql
   mysql -u root -p dbperfume < create_product_variants_table.sql
   ```

## Production Configuration

### Environment Variables

Set these in your production environment:

```bash
# Database
DB_CONNECTION_STRING="Server=your-db-host;Database=dbperfume;User=your-user;Password=your-password;"

# Email
SMTP_SERVER="smtp.gmail.com"
SMTP_PORT="587"
SMTP_USERNAME="your-email@gmail.com"
SMTP_PASSWORD="your-app-password"

# Payments
VNPAY_TMN_CODE="your-tmn-code"
VNPAY_HASH_SECRET="your-hash-secret"
MOMO_PARTNER_CODE="your-partner-code"
MOMO_ACCESS_KEY="your-access-key"
MOMO_SECRET_KEY="your-secret-key"
```

### SSL Configuration

For HTTPS in production:

1. **Using Docker with reverse proxy (recommended)**
   ```yaml
   # Add to docker-compose.yml
   nginx:
     image: nginx:alpine
     ports:
       - "80:80"
       - "443:443"
     volumes:
       - ./nginx.conf:/etc/nginx/nginx.conf
       - ./ssl:/etc/ssl/certs
     depends_on:
       - guhastore-web
   ```

2. **Using ASP.NET Core HTTPS**
   ```bash
   # In Docker
   ASPNETCORE_URLS="https://+:443;http://+:80"
   ASPNETCORE_Kestrel__Certificates__Default__Path=/https/your-cert.pfx
   ASPNETCORE_Kestrel__Certificates__Default__Password=your-cert-password
   ```

## Database Setup

### Using Docker MySQL
The `docker-compose.yml` includes MySQL 8.0 with automatic initialization.

### Using External MySQL
1. Create database: `dbperfume`
2. Import schema:
   ```sql
   mysql -u root -p dbperfume < dbperfume.sql
   mysql -u root -p dbperfume < create_product_variants_table.sql
   ```
3. Update connection string in appsettings.Production.json

## File Uploads

The application stores uploaded files in `wwwroot/uploads/`. In production:

- Use persistent volumes for Docker
- Configure cloud storage (AWS S3, Azure Blob, etc.) for scalability
- Set up CDN for faster image delivery

## Monitoring and Logging

### Health Checks
Access `/health` endpoint to monitor application health.

### Logging
- Application logs are written to `logs/guhastore-.txt`
- Configure log rotation and monitoring
- Consider centralized logging (ELK stack, Seq, etc.)

## Security Considerations

1. **Change default passwords**
2. **Use strong database passwords**
3. **Enable HTTPS only**
4. **Configure firewall rules**
5. **Regular security updates**
6. **Monitor for vulnerabilities**

## Backup Strategy

### Database Backups
```bash
# Daily backup script
mysqldump -u user -p password dbperfume > backup_$(date +%Y%m%d).sql

# Automated with cron
0 2 * * * /path/to/backup-script.sh
```

### File Backups
```bash
# Backup uploads directory
tar -czf uploads_backup_$(date +%Y%m%d).tar.gz wwwroot/uploads/
```

## Scaling

### Horizontal Scaling
- Use load balancer (nginx, HAProxy)
- Multiple app instances
- Shared database and file storage

### Vertical Scaling
- Increase container resources
- Optimize database queries
- Use caching (Redis)

## Troubleshooting

### Common Issues

1. **Database connection fails**
   - Check connection string
   - Verify MySQL is running
   - Check firewall rules

2. **File uploads fail**
   - Check file permissions
   - Verify upload directory exists
   - Check file size limits

3. **Payment gateways not working**
   - Verify credentials
   - Check callback URLs
   - Review gateway-specific documentation

### Logs Location
- Application logs: `logs/guhastore-.txt`
- Docker logs: `docker logs guhastore-web`
- MySQL logs: `docker logs guhastore-db`

## Support

For deployment issues, check:
1. Application logs
2. Docker container logs
3. Database connectivity
4. Network configuration

---

## Environment Checklist

- [ ] Domain configured
- [ ] SSL certificate installed
- [ ] Database created and populated
- [ ] Email service configured
- [ ] Payment gateways set up
- [ ] File upload directories created
- [ ] Environment variables set
- [ ] Firewall configured
- [ ] Backup strategy implemented
- [ ] Monitoring set up
