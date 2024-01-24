export function initializeImagesSlider(
    imagesRef = new Element(),
    imageControlsRef = new Element())
{
    const scrollWidth = imagesRef.scrollWidth;
    const indexShowers = imageControlsRef.querySelectorAll('.index-shower');
    const ammOfImages = imagesRef.children.length;

    let selectedIndexShower = indexShowers[0];
    selectedIndexShower.classList.add('selected');

    //On scroll:
    imagesRef.addEventListener('scroll', () => {
        const scrollAmm = imagesRef.scrollLeft;
        const viewedImageIndex = Math.floor(scrollAmm / scrollWidth * ammOfImages);
      
        //Check if we've changed view
        const currentlyViewedIndexShower = indexShowers[viewedImageIndex];
        if (currentlyViewedIndexShower === selectedIndexShower)
            return;

        //Deselect
        selectedIndexShower.classList.remove('selected');

        //Select
        selectedIndexShower = indexShowers[viewedImageIndex];
        selectedIndexShower.classList.add('selected');
    });
}