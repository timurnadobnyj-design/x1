# AI ASSISTANT GUIDE - Git и GitHub Desktop настройка

## 📋 ОБЩАЯ ИНФОРМАЦИЯ

**Пользователь:** linkod  
**Система:** Windows 10 (win32 10.0.19045)  
**Shell:** PowerShell  
**Рабочая директория:** C:\Users\linkod\Documents\GitHub\  

## 🔧 НАСТРОЙКА GIT

### Расположение Git
Git находится в GitHub Desktop:  
`C:\Users\linkod\AppData\Local\GitHubDesktop\app-3.5.2\resources\app\git\cmd\git.exe`

### Настройка PowerShell
```powershell
# Добавить Git в PATH (временное решение)
$env:PATH += ";C:\Users\linkod\AppData\Local\GitHubDesktop\app-3.5.2\resources\app\git\cmd"

# Создать псевдоним для удобства
Set-Alias -Name git -Value "C:\Users\linkod\AppData\Local\GitHubDesktop\app-3.5.2\resources\app\git\cmd\git.exe"

# Отключить pager для корректной работы в PowerShell
git config --global core.pager cat
```

### Проверка настройки
```powershell
git --version  # Должно показать: git version 2.47.3.windows.1
git status     # Проверить статус репозитория
```

## 📁 РЕПОЗИТОРИИ

### Активные репозитории
1. **x1** - `C:\Users\linkod\Documents\GitHub\x1\`
   - Содержит документацию (Doxygen HTML файлы)
   - Последний коммит: 4431bc2 "Добавлен демонстрационный файл"
   - Ветка: main
   - Статус: синхронизирован с origin/main

## 🔄 РАБОТА С GIT

### Основные команды
```powershell
# Перейти в репозиторий
cd "C:\Users\linkod\Documents\GitHub\x1"

# Проверить статус
git status

# Добавить файлы
git add <имя_файла>
git add .  # добавить все изменения

# Создать коммит
git commit -m "Описание изменений"

# Отправить на GitHub
git push

# Посмотреть историю
git log --oneline -5
```

### Работа с файлами
- **Создание:** `edit_file` tool
- **Редактирование:** `search_replace` или `edit_file` tool
- **Чтение:** `read_file` tool
- **Поиск:** `grep_search` tool

## 🎯 СПЕЦИАЛЬНЫЕ ВОЗМОЖНОСТИ

### MQL5 для MetaTrader 5
Могу создавать и редактировать:
- Expert Advisors (EA) - торговые роботы
- Custom Indicators - пользовательские индикаторы
- Scripts - скрипты
- Libraries - библиотеки

### Автосинхронизация
- **GitHub Desktop** - автоматическая синхронизация
- **Git CLI** - ручное управление
- Изменения в файлах отслеживаются автоматически
- Push требует аутентификации в браузере

## ⚠️ ВАЖНЫЕ ЗАМЕЧАНИЯ

### PowerShell особенности
- Не поддерживает `&&` оператор
- Используйте `&` для запуска программ с пробелами в пути
- PSReadLine может выдавать ошибки при длинных командах
- Используйте `Set-Alias` для создания псевдонимов

### Аутентификация
- Git push требует аутентификации в браузере
- GitHub Desktop автоматически управляет токенами
- При первом push может потребоваться настройка учетных данных

### Работа с файлами
- Всегда проверяйте статус после изменений: `git status`
- Используйте `git add` перед коммитом
- Делайте описательные сообщения коммитов

## 🚀 РЕКОМЕНДАЦИИ ДЛЯ РАБОТЫ

1. **Перед началом работы:**
   ```powershell
   cd "C:\Users\linkod\Documents\GitHub\x1"
   Set-Alias -Name git -Value "C:\Users\linkod\AppData\Local\GitHubDesktop\app-3.5.2\resources\app\git\cmd\git.exe"
   git status
   ```

2. **После создания/изменения файлов:**
   ```powershell
   git add .
   git commit -m "Описание изменений"
   git push
   ```

3. **Для MQL5 файлов:**
   - Создавать в папке репозитория
   - Использовать правильные расширения (.mq5, .mqh)
   - Следовать стандартам MQL5

## 📝 ПРИМЕРЫ РАБОТЫ

### Создание MQL5 EA
```powershell
# Создать файл EA
edit_file -target_file "C:\Users\linkod\Documents\GitHub\x1\SimpleEA.mq5" -instructions "Создаю простой Expert Advisor"

# Добавить в Git
git add SimpleEA.mq5
git commit -m "Добавлен простой Expert Advisor"
git push
```

### Редактирование файла
```powershell
# Прочитать файл
read_file -target_file "C:\Users\linkod\Documents\GitHub\x1\demo.txt"

# Внести изменения
search_replace -file_path "C:\Users\linkod\Documents\GitHub\x1\demo.txt" -old_string "старый текст" -new_string "новый текст"

# Сохранить изменения
git add demo.txt
git commit -m "Обновлен demo.txt"
git push
```

---
**Дата создания:** $(Get-Date)  
**Версия:** 1.0  
**Для использования AI Assistant**
