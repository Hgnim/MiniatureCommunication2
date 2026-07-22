//根据不同的页面加载不同的模块
const pageMap: Record<string, () => Promise<any>> = {
    Admin_Index: () => import('./ts/Admin/index'),
    Talk_Index: () => import('./ts/Talk/index'),
    Home_Index:()=>import('./ts/Home/index'),
};

//根据head上的data-page属性来决定加载哪个模块
const pageKey = document.head.dataset.page ?? 'unknown';
if (pageMap[pageKey]) {
    pageMap[pageKey]().then(mod => {
        mod.init();
    });
} else {
    console.warn(`[bundle] 未找到 ${pageKey} 对应的模块`);
}