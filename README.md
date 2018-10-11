# hand-game
Test task for interview
Реализуйте игру камень-ножницы-бумага используя Unity

Результатом тестового задания должен быть полный Unity проект в котором есть Start сцена, запуск которой в редакторе стартует игру.

## Игровая динамика
Игра происходит по раундам, конца игры не предусмотрено, количество раундов не ограничено.
Текущий счет по раундам (например 2 : 1) должен постоянно отображаться на экране.
Каждый раунд состоит из двух этапов
1. на экране три кнопки позволяющие игроку выбрать одну из трех фигур камень-ножницы-бумага. Нажатие на любую из трех кнопок фиксирует выбор фигуры игроком и переходит к фазе 2.
2. на экране последовательно отображаются:
a. выбор фигуры оппонентом (AI)
b. результат раунда (победа-поражение)
c. изменение счета по раундам (например 3 : 4  -> 4 : 4)
d. по действию пользователя (например клик на специальную кнопку либо в любом месте экрана) начинается новый раунд

## Режимы игры
Игра должна уметь поддерживать два режима
* "честный", когда выбор фигуры оппонентом никак не связан с выбором игрока
* "нечестный", в этом случае оппонент должен побеждать в соответствии с заданной вероятностью P (число [0..1]) - т.е. при P=1 оппонент всегда побеждает, при P=0.5 - примерно в половине раундов и т.д.

## Настройки
Выбор режима и вероятность выигрыша AI для "нечестного" режима - можно реализовать любым удобным способом. Возможностью менять конфигурацию в процессе игры можно пренебречь. 
