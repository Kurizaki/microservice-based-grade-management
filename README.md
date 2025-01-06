# Microservice-based Grade Management System

## Project Overview

**Project Name:**  
Microservice-based Grade Management System

**Description:**  
The aim of this project is to develop a distributed application for managing grades using a microservice architecture. The system consists of three independent services:

1. **Login Service**: Manages user authentication and authorization.
2. **Grade Service**: Handles storage and management of grades and categories.
3. **Calculation Service**: Performs grade-related calculations, such as average grade computation.

To ensure efficient development and deployment, the project employs **DevOps practices** like **CI/CD pipelines**, **containerization**, and **orchestration** using tools such as **Docker** and **Kubernetes**.

---

## Project Objectives

1. **Login Service**: 
   - User management and authentication using **JWT** (JSON Web Tokens) and a dedicated database.

2. **Grade Service**: 
   - Store, manage, and update grades in a separate database.

3. **Calculation Service**: 
   - Compute grade averages and other calculations upon user requests.

4. **DevOps Integration**:
   - Implementation of **CI/CD pipelines** for automatic deployment of the microservices and containerization with **Docker**.

5. **Monitoring**:
   - System monitoring using tools like **Prometheus** and **ELK Stack** to ensure high availability and performance. 

---

## Technical Details

### 1. Login Service
- **Purpose**: Handles user authentication and stores user information.
- **Database**: Stores user data including encrypted passwords and roles.
- **Key API Endpoints**:
  - `POST /login`: User login, returns JWT token.
  - `POST /register`: User registration.
  - `POST /validate`: Validates JWT token for authentication.

### 2. Grade Service
- **Purpose**: Manages grades and categories, linked to users.
- **Database**: Stores grades and category data associated with user IDs from the Login Service.
- **Key API Endpoints**:
  - `POST /noten`: Create a new grade.
  - `GET /noten/GettGradesFromUser/`: Retrieve grades for a specific user.
  - `PUT /noten/{id}`: Update a grade.
  - `DELETE /noten/{id}`: Delete a grade.

### 3. Calculation Service
- **Purpose**: Performs grade calculations, like averages, based on data from the Grade Service.
- **Key API Endpoints**:
  - `POST /berechnen/durchschnitt`: Calculates the average grade for a user.
  - `POST /berechnen/kategorie`: Calculates category-specific averages or other statistics.

---

## Architecture

- **Microservice Design**: Each service (Login, Grade, Calculation) is decoupled and manages its own database. Communication occurs via REST APIs.
- **Security**: JWT-based authentication ensures secure access to services.
- **DevOps & Scalability**: Services are containerized and orchestrated with **Kubernetes**, allowing for independent scaling and maintenance.
  
### Example Workflow:
1. User logs in via the **Login Service** and receives a JWT token.
2. The JWT token is used to interact with the **Grade Service** for managing grades.
3. The **Calculation Service** accesses the Grade Service to compute results like average grades.

---

## DevOps Integration

- **CI/CD Pipeline**: Continuous integration and deployment on own server.
- **Containerization**: Services are containerized using **Docker** for consistent deployment.
- **Orchestration**: Kubernetes orchestrates the containers to ensure scalability and resilience.
- **Monitoring**: Microservices are monitored using **Prometheus** and logs are managed with the **ELK Stack** for better system reliability.

---

## Modules & Competencies

### Module 324: Support DevOps Processes with Tools
- Competency 1: Build a **CI/CD pipeline** with Jenkins.
- Competency 2: **Containerize** services using Docker and manage with Kubernetes.
- Competency 3: Implement **API communication** between services.
- Competency 4: Set up **monitoring** and logging for the microservices.

### Module 321: Analyze and Understand Distributed Systems
- Competency 1: Analyze and implement a **microservices architecture**.
- Competency 2: Use **APIs** for service communication.

---

## Benefits of the Architecture
- **Security**: JWT tokens and isolated databases ensure secure handling of user information and grades.
- **Scalability**: Each service can be scaled independently as per the load and demand.
- **Maintainability**: Clear separation of concerns between services simplifies maintenance and future expansion.

---

## Tools & Technologies

- **Development Environment**: Any environment supporting Docker and Kubernetes.
- **Database**: **SQLite** for Login, Grade, and Calculation services.
- **Version Control**: **GitHub** repository for code management.
- **Monitoring Tools**: **Prometheus** and **ELK Stack** for real-time monitoring and log management.

---

## Team & Timeline

- **Project Leader**: Keanu Koelewjin  
- **Team Members**: Pascal Oestrich, Timo Goedertier, Stefan Jesenko, Julius Burlet

- **Project Start Date**: September 10, 2024  
- **Project End Date**: January 13, 2025  


