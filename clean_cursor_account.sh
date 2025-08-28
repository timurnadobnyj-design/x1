#!/bin/bash

# Скрипт для полного удаления данных старого аккаунта Cursor AI на Linux

echo "========================================="
echo "Очистка данных аккаунта Cursor AI"
echo "========================================="
echo ""
echo "ВНИМАНИЕ: Этот скрипт удалит ВСЕ данные Cursor, включая:"
echo "- Настройки и конфигурации"
echo "- Токены авторизации"
echo "- Кэш и временные файлы"
echo "- Историю и логи"
echo ""
read -p "Вы уверены, что хотите продолжить? (y/n): " -n 1 -r
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]
then
    echo "Отменено пользователем."
    exit 1
fi

echo ""
echo "Начинаем очистку..."
echo ""

# 1. Закрываем Cursor, если он запущен
echo "1. Закрываем процессы Cursor..."
pkill -f cursor 2>/dev/null
pkill -f Cursor 2>/dev/null
sleep 2

# 2. Удаляем основную директорию данных VM daemon
echo "2. Удаляем данные VM daemon Cursor..."
if [ -d "$HOME/.vm-daemon/vm-daemon-cursor-data" ]; then
    rm -rf "$HOME/.vm-daemon/vm-daemon-cursor-data"
    echo "   ✓ Удалено: ~/.vm-daemon/vm-daemon-cursor-data"
fi

# 3. Удаляем бинарные файлы VM daemon
echo "3. Удаляем бинарные файлы Cursor..."
for dir in $HOME/.vm-daemon/bin/vm-daemon-cursor-*; do
    if [ -d "$dir" ]; then
        rm -rf "$dir"
        echo "   ✓ Удалено: $dir"
    fi
done

# 4. Удаляем конфигурации из ~/.config
echo "4. Удаляем конфигурации Cursor..."
if [ -d "$HOME/.config/Cursor" ]; then
    rm -rf "$HOME/.config/Cursor"
    echo "   ✓ Удалено: ~/.config/Cursor"
fi

if [ -d "$HOME/.config/Cursor Nightly" ]; then
    rm -rf "$HOME/.config/Cursor Nightly"
    echo "   ✓ Удалено: ~/.config/Cursor Nightly"
fi

# 5. Удаляем директории в домашней папке
echo "5. Удаляем директории Cursor из домашней папки..."
if [ -d "$HOME/.cursor-nightly" ]; then
    rm -rf "$HOME/.cursor-nightly"
    echo "   ✓ Удалено: ~/.cursor-nightly"
fi

if [ -d "$HOME/.cursor-server" ]; then
    rm -rf "$HOME/.cursor-server"
    echo "   ✓ Удалено: ~/.cursor-server"
fi

if [ -d "$HOME/.cursor" ]; then
    rm -rf "$HOME/.cursor"
    echo "   ✓ Удалено: ~/.cursor"
fi

# 6. Удаляем кэш
echo "6. Удаляем кэш Cursor..."
if [ -d "$HOME/.cache/cursor" ]; then
    rm -rf "$HOME/.cache/cursor"
    echo "   ✓ Удалено: ~/.cache/cursor"
fi

if [ -d "$HOME/.cache/cursor-nightly" ]; then
    rm -rf "$HOME/.cache/cursor-nightly"
    echo "   ✓ Удалено: ~/.cache/cursor-nightly"
fi

# 7. Удаляем данные из local share
echo "7. Удаляем данные из local share..."
if [ -d "$HOME/.local/share/cursor" ]; then
    rm -rf "$HOME/.local/share/cursor"
    echo "   ✓ Удалено: ~/.local/share/cursor"
fi

if [ -d "$HOME/.local/share/cursor-nightly" ]; then
    rm -rf "$HOME/.local/share/cursor-nightly"
    echo "   ✓ Удалено: ~/.local/share/cursor-nightly"
fi

# 8. Очищаем временные файлы
echo "8. Удаляем временные файлы..."
rm -rf /tmp/cursor* 2>/dev/null
rm -rf /tmp/Cursor* 2>/dev/null
echo "   ✓ Очищены временные файлы"

# 9. Очищаем логи
echo "9. Удаляем логи..."
if [ -d "$HOME/.vm-daemon/logs" ]; then
    find "$HOME/.vm-daemon/logs" -name "*cursor*" -o -name "*Cursor*" | xargs rm -rf 2>/dev/null
    echo "   ✓ Очищены логи Cursor"
fi

echo ""
echo "========================================="
echo "✅ Очистка завершена!"
echo "========================================="
echo ""
echo "Все данные старого аккаунта Cursor удалены."
echo "Теперь вы можете:"
echo "1. Перезапустить Cursor"
echo "2. Войти с новым аккаунтом"
echo ""
echo "Если Cursor установлен через пакетный менеджер,"
echo "вы можете переустановить его командой:"
echo "  - Для snap: snap remove cursor && snap install cursor"
echo "  - Для AppImage: скачайте новую версию с сайта"
echo ""