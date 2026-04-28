namespace Game
{
    public enum CellState
    {
        Empty,        // клетка свободна
        Occupied,     // в клетке стабильно стоит элемент
        Falling,      // элемент падает в эту клетку (логически — здесь, визуально — летит)
        Destroying    // элемент в процессе уничтожения
    }
}