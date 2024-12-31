#!/bin/bash

# Create log directories with proper permissions
sudo mkdir -p /var/log/gradesystem/{backend,frontend,system}

# Set ownership to syslog user and adm group
sudo chown -R syslog:adm /var/log/gradesystem

# Set proper permissions (770 allows group write access)
sudo chmod -R 770 /var/log/gradesystem

# Create log files with proper permissions
touch_log() {
    sudo touch "/var/log/gradesystem/$1"
    sudo chown syslog:adm "/var/log/gradesystem/$1"
    sudo chmod 660 "/var/log/gradesystem/$1"
}

# Backend logs
touch_log "backend/errors.log"
touch_log "backend/access.log" 
touch_log "backend/changes.log"

# Frontend logs
touch_log "frontend/errors.log"
touch_log "frontend/build.log"

# System logs
touch_log "system/docker.log"
touch_log "system/security.log"

echo "Logging system setup complete"
