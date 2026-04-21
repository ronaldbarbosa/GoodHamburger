(function () {
    window.themeManager = {
        init: function (dotnetRef) {
            this.dotnetRef = dotnetRef;
            
            const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
            
            const handleChange = (e) => {
                const savedTheme = localStorage.getItem('theme_preference');
                if (!savedTheme || savedTheme === 'system') {
                    if (this.dotnetRef) {
                        this.dotnetRef.invokeMethodAsync('SetThemeFromSystem', e.matches);
                    }
                }
            };
            
            mediaQuery.addEventListener('change', handleChange);
        },
        
        isDarkMode: function () {
            return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        }
    };
})();
