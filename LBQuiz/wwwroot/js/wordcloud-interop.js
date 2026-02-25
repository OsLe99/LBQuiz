window.renderWordCloud = (canvasId, wordData) => {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;
    
    canvas.width = canvas.offsetWidth;
    canvas.height = canvas.offsetHeight;
    
    WordCloud(canvas,{
        list: wordData,
        gridSize: 8,
        weightFactor: 24,
        fontFamily: 'Arial',
        color: 'random-dark',
        rotateRatio: 0.3,
        backgroundColor: '#ffffff'
    });
};