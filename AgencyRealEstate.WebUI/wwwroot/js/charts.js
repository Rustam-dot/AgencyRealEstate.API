window.drawCharts = (statusData, typeData) => {
    // Удаляем старые канвасы, если они существуют (на случай повторного вызова)
    const statusCanvas = document.getElementById('statusChart');
    const typeCanvas = document.getElementById('typeChart');

    if (!statusCanvas || !typeCanvas) return;

    // Уничтожаем существующие графики, чтобы избежать наложения
    if (statusCanvas._chart) statusCanvas._chart.destroy();
    if (typeCanvas._chart) typeCanvas._chart.destroy();

    // График по статусам
    const statusCtx = statusCanvas.getContext('2d');
    statusCanvas._chart = new Chart(statusCtx, {
        type: 'doughnut',
        data: {
            labels: Object.keys(statusData),
            datasets: [{
                data: Object.values(statusData),
                backgroundColor: [
                    '#2ecc71', // Available – зеленый
                    '#f39c12', // Reserved – золотой
                    '#e74c3c', // Sold – красный
                    '#3498db', // Rented – синий
                    '#95a5a6'  // Inactive – серый
                ],
                borderColor: '#121212',
                borderWidth: 3,
                hoverBorderColor: '#f39c12'
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        color: '#ccc',
                        padding: 20,
                        font: {
                            family: 'Inter',
                            size: 12
                        }
                    }
                },
                title: {
                    display: true,
                    text: 'Распределение объектов по статусам',
                    color: '#f39c12',
                    font: {
                        family: 'Inter',
                        size: 16,
                        weight: 'bold'
                    }
                }
            }
        }
    });

    // График по типам
    const typeCtx = typeCanvas.getContext('2d');
    typeCanvas._chart = new Chart(typeCtx, {
        type: 'pie',
        data: {
            labels: Object.keys(typeData),
            datasets: [{
                data: Object.values(typeData),
                backgroundColor: [
                    '#9b59b6', // Квартира
                    '#1abc9c', // Дом
                    '#e67e22', // Таунхаус
                    '#34495e', // Коммерческая
                    '#e74c3c'  // Участок
                ],
                borderColor: '#121212',
                borderWidth: 3,
                hoverBorderColor: '#f39c12'
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        color: '#ccc',
                        padding: 20,
                        font: {
                            family: 'Inter',
                            size: 12
                        }
                    }
                },
                title: {
                    display: true,
                    text: 'Распределение по типам недвижимости',
                    color: '#f39c12',
                    font: {
                        family: 'Inter',
                        size: 16,
                        weight: 'bold'
                    }
                }
            }
        }
    });
};