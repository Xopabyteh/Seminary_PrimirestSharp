var MyPhotosPage = MyPhotosPage || {};
MyPhotosPage.initializeVirtualizationScrollChecker = function(
    photosGridRef = new Element(),
    dotNetHelper) 
{
    const scrollMargin = 100;
    photosGridRef.addEventListener('scroll', (e) => {
        const scrollableHeight = photosGridRef.scrollHeight - photosGridRef.clientHeight;
        const scrollY = photosGridRef.scrollTop;

        if (scrollY > scrollableHeight - scrollMargin) {
            //Load more photos
            MyPhotosPage.loadMorePhotos(dotNetHelper);
        }
    });
}

MyPhotosPage.loadMorePhotos = function (dotNetHelper) {
    //Invoke .NET method
    dotNetHelper.invokeMethodAsync('LoadMorePhotos');
}