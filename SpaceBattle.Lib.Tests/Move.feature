Функция: Движение

    Сценарий: Игровой объект может перемещаться по прямой 
        Дано космический корабль находится в точке пространства с координатами (12, 5)
        И имеет мгновенную скорость (-5, 3)
        Когда происходит прямолинейное равномерное движение без деформации
        Тогда космический корабль перемещается в точку пространства с координатами (7, 8) 
    
    Сценарий: Если невозможно определить текущее положение игрового объекта в пространстве, то перемещение по прямой невозможно 
        Дано космический корабль, положение в пространстве которого невозможно определить
        И имеет мгновенную скорость (-5, 3)
        Когда происходит прямолинейное равномерное движение без деформации
        Тогда возникает ошибка Exception 

    Сценарий: Если невозможно определить мгновенную скорость игрового объекта, то перемещение по прямой невозможно 
        Дано космический корабль находится в точке пространства с координатами (12, 5)
        И скорость корабля определить невозможно
        Когда происходит прямолинейное равномерное движение без деформации
        Тогда возникает ошибка Exception 

    Сценарий: Если невозможно изменить положение игрового объекта в пространстве, то перемещение по прямой невозможно 
        Дано космический корабль находится в точке пространства с координатами (12, 5)
        И имеет мгновенную скорость (-5, 3)
        И изменить положение в пространстве космического корабля невозможно
        Когда происходит прямолинейное равномерное движение без деформации
        Тогда возникает ошибка Exception
