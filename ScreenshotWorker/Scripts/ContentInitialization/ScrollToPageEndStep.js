(scrollDelay, waitForPossibleContentLoad, maxExecutionTimeout, maxRenderedHeight) => {
    {
        const scrollStep = window.innerHeight / 5;

        const scrollPromise = new Promise(resolve => {
            {
                let clearIntervalId;
                window.__scrollCompleted = false;

                window.__scrollInterval = setInterval(() => {
                    {
                        const scrolled = window.scrollY;
                        const currentScrolledHeight = window.innerHeight + scrolled;
                        const atBottom = (currentScrolledHeight + scrollStep) >= document.documentElement.scrollHeight;
                        const maxHeightReached = maxRenderedHeight <= currentScrolledHeight;

                        if (maxHeightReached)
                            return clearIntervalId && clearTimeout(clearIntervalId);

                        if (clearIntervalId && atBottom)
                            return;

                        if (clearIntervalId)
                            clearTimeout(clearIntervalId);

                        if (atBottom) {
                            {
                                clearIntervalId = setTimeout(() => {
                                    {
                                        clearInterval(window.__scrollInterval);
                                        window.__scrollCompleted = true;
                                        resolve(true);
                                    }
                                }, waitForPossibleContentLoad);
                            }
                        } else {
                            {
                                window.scrollBy(0, scrollStep);
                            }
                        }
                    }
                }, scrollDelay);
            }
        });

        const timeoutPromise = new Promise((_, reject) =>
            setTimeout(() => reject('Scroll script timeout exceeded'), maxExecutionTimeout)
        );

        return Promise.race([scrollPromise, timeoutPromise]);
    }
};