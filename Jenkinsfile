pipeline {
    agent any

    environment {
        DOTNET_VERSION = "8.0.x"
    }

    triggers {
        //Every commit to feature/module9-cicd
        pollSCM('H/5 * * * *')
        //Daily run at 02:00AM
        cron('H 2 * * *')
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'feature/module9-cicd', url: 'https://github.com/BuchRepository/com.epam.rp.git'
            }
        }

        stage('Setup .NET') {
            steps {
                echo "Setting up .NET SDK..."
                sh 'dotnet --version || brew install dotnet'
            }
        }

        stage('Restore dependencies') {
            steps {
                echo "Restoring dependencies..."
                sh 'dotnet restore com.epam.rp.sln'
            }
        }

        stage('Build solution') {
            steps {
                echo "Building project..."
                sh 'dotnet build com.epam.rp.sln --configuration Release --no-restore'
            }
        }

        stage('Run API tests') {
            steps {
                echo "Running API tests..."
                sh '''
                    dotnet test com.epam.rp.api/com.epam.rp.api.csproj \
                    --configuration Release \
                    --logger "trx;LogFileName=api_test_results.trx"
                '''
            }
            post {
                always {
                    junit '**/api_test_results.trx'
                }
            }
        }

        stage('Run UI tests') {
            steps {
                echo "Running UI tests..."
                sh '''
                    dotnet test com.epam.rp.ui/com.epam.rp.ui.csproj \
                    --configuration Release \
                    --logger "trx;LogFileName=ui_test_results.trx"
                '''
            }
            post {
                always {
                    junit '**/ui_test_results.trx'
                }
            }
        }
    }

    post {
        success {
            echo 'All tests passed successfully!'
        }
        failure {
            echo 'Some tests failed.'
        }
        always {
            echo 'Build and test pipeline finished.'
        }
    }
}
