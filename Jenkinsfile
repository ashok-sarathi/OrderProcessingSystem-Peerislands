pipeline {
    agent any

    environment {
        DOTNET_VERSION = '9.0'
        IMAGE_NAME = 'yourdockerhubusername/yourapp'
        IMAGE_TAG = "${GIT_COMMIT}"
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test --configuration Release --no-build --verbosity normal'
            }
        }

        stage('Publish') {
            steps {
                sh 'dotnet publish -c Release -o publish'
            }
        }

        stage('Build Docker Image') {
            steps {
                sh 'docker build -t $IMAGE_NAME:$IMAGE_TAG .'
            }
        }
    }

    post {
        success {
            echo 'Build, Test, and Docker Image creation successful!'
        }
        failure {
            echo 'Pipeline failed. Check logs.'
        }
    }
}
