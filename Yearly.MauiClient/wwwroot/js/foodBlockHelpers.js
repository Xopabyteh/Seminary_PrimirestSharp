var FoodBlock = FoodBlock || {};
FoodBlock.initializeImagesSlider = function(
    imagesRef = new Element(),
    imageControlsRef = new Element())
{
    const scrollWidth = imagesRef.scrollWidth;
    const indexShowers = imageControlsRef.querySelectorAll('.index-shower');
    //const ammOfImages = imagesRef.children.length;

    let selectedIndexShower = indexShowers[0];
    selectedIndexShower.classList.add('selected');

    //On scroll:
    imagesRef.addEventListener('scroll', () => {
        const scrollAmm = imagesRef.scrollLeft;
        const viewedImageIndex = Math.round(scrollAmm / scrollWidth);

        //Check if we've changed view
        const currentlyViewedIndexShower = indexShowers[viewedImageIndex];
        if (currentlyViewedIndexShower === selectedIndexShower)
            return;

        //Deselect
        selectedIndexShower.classList.remove('selected');

        //Select
        selectedIndexShower = currentlyViewedIndexShower;
        selectedIndexShower.classList.add('selected');
    });
}