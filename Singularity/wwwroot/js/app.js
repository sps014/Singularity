let observer;
window.subscribeObserver = function (el, dotnet) {
    observer = new IntersectionObserver(callback.bind(dotnet), {
        root: document.getElementById("tabViewBody"),
        rootMargin: "0px",
        threshold: 0.01,
    });

    observer.observe(el);
}
window.unsubscribeObserver = function()
{
    if (observer != undefined)
        observer.disconnect();
}
function callback(e) {
    for (let entry of e) {
        const visible = entry.intersectionRatio != 0;
        this.invokeMethodAsync("visibiltyChanged", visible);
        //console.log(this.i+"  -> "+entry.intersectionRatio);
    }
}


window.makeItPressable = (el, dotnet) => {
    el.addEventListener('long-press', function (e) {
        dotnet.invokeMethodAsync("longPress");
    });
};


