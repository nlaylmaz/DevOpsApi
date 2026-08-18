#!/bin/bash

APP_ENV="Development"

prepare_app() {
    echo "Uygulama hazırlanıyor..."
    echo "Environment: $APP_ENV"
}

if [ "$APP_ENV" = "Development" ]; then
    echo "Development modunda çalışacak."
else
    echo "Development dışında bir ortam."
fi

for i in 1 2 3
do
    echo "Hazırlık adımı $i"
done

prepare_app

dotnet run

if [ $? -eq 0 ]; then
    echo "API başarıyla kapandı."
else
    echo "API çalıştırılırken hata oluştu."
    exit 1
fi
