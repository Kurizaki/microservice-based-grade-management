# Use nginx as base image
FROM nginx:alpine

# Copy the frontend files to nginx html directory
COPY . /usr/share/nginx/html/

# Expose port
EXPOSE 8080
EXPOSE 8081

# Start nginx
CMD ["nginx", "-g", "daemon off;"]
