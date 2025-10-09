pipeline {
    agent any

    environment {
        DOTNET_VERSION = "8.0.x"
    }

    triggers {
        //Every commit to develop
        pollSCM('H/5 * * * *')
        //Daily run at 02:00AM
        cron('H 2 * * *')
    }

    stages {
        stage('Checkout') {
            steps {
                git branch: 'develop', url: 'https://github.com/BuchRepository/com.epam.rp.git'
            }
        }

        stage('Setup .NET') {
            steps {
                echo "Checking .NET SDK installation..."
                sh '/usr/local/share/dotnet/dotnet --version'
                sh '/usr/local/share/dotnet/dotnet --info'
            }
        }

        stage('Inject credentials') {
            steps {
                withCredentials([
                    string(credentialsId: 'LOGIN', variable: 'LOGIN'),
                    string(credentialsId: 'PASSWORD', variable: 'PASSWORD'),
                    string(credentialsId: 'API_TOKEN', variable: 'API_TOKEN')
                ]) {
                    echo "Environment variables injected."
                    sh '''
                        echo "LOGIN=$LOGIN"
                        echo "PASSWORD=[HIDDEN]"
                        echo "API_TOKEN=[HIDDEN]"
                    '''
                }
            }
        }

        stage('Restore dependencies') {
            steps {
                echo "Restoring dependencies..."
                sh '/usr/local/share/dotnet/dotnet restore com.epam.rp.sln'
            }
        }

        stage('Build solution') {
            steps {
                echo "Building project..."
                sh '/usr/local/share/dotnet/dotnet build com.epam.rp.sln --configuration Release --no-restore'
            }
        }

        stage('Run API tests') {
            environment {
                LOGIN = credentials('LOGIN')
                PASSWORD = credentials('PASSWORD')
                API_TOKEN = credentials('API_TOKEN')
            }
            steps {
                echo "Running API tests..."
                sh '''
                    /usr/local/share/dotnet/dotnet test com.epam.rp.api/com.epam.rp.api.csproj \
                    --configuration Release
                '''
            }
        }

        stage('Upload API Report') {
            steps {
                echo 'Uploading API report as artifact...'
                archiveArtifacts artifacts: 'com.epam.rp.api/bin/Release/net8.0/index.html', fingerprint: true
            }
        }


        stage('Run UI tests') {
            environment {
                LOGIN = credentials('LOGIN')
                PASSWORD = credentials('PASSWORD')
                API_TOKEN = credentials('API_TOKEN')
            }
            steps {
                echo "Running UI tests..."
                sh '''
                    /usr/local/share/dotnet/dotnet test com.epam.rp.ui/com.epam.rp.ui.csproj \
                    --configuration Release
                '''
            }
        }

	stage('Upload UI Report') {
            steps {
                archiveArtifacts artifacts: 'com.epam.rp.ui/bin/Release/net8.0/index.html', fingerprint: true
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
