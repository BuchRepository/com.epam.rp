pipeline {
    agent any

    environment {
        DOTNET_VERSION = "8.0.x"
    }

    triggers {
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
                    string(credentialsId: 'API_TOKEN', variable: 'API_TOKEN'),
                    string(credentialsId: 'SONAR_QUBE_TOKEN', variable: 'SONAR_QUBE_TOKEN')
                ]) {
                    echo "Environment variables injected."
                    sh '''
                        echo "LOGIN=$LOGIN"
                        echo "PASSWORD=[HIDDEN]"
                        echo "API_TOKEN=[HIDDEN]"
                        echo "SONAR_QUBE_TOKEN=[HIDDEN]"
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

        stage('SonarQube Analysis') {
            steps {
                withCredentials([string(credentialsId: 'SONAR_QUBE_TOKEN', variable: 'SONAR_QUBE_TOKEN')]) {
                    sh """
                        dotnet sonarscanner begin /o:"buchrepository" /k:"BuchRepository_com.epam.rp" /d:sonar.token="$SONAR_QUBE_TOKEN"
                        dotnet build
                        dotnet sonarscanner end /d:sonar.login="$SONAR_QUBE_TOKEN"
                    """
                }
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
