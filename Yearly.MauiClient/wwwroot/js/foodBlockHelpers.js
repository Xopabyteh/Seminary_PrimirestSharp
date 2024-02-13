var FoodBlock = FoodBlock || {};
FoodBlock.initializeImagesSlider = function (
    imagesRef = new Element(),
    imageControlsRef = new Element())
{
    //Disable manual scrolling
    imagesRef.addEventListener('scroll', (e) => {
        e.preventDefault();
        e.stopPropagation();

        return false;
    });

    //Setup onclick scrolling
    const images = imagesRef.children;
    const ammOfImages = images.length;
    const indexShowers = imageControlsRef.querySelectorAll('.index-shower');

    let selectedImageI = 0;
    indexShowers[selectedImageI].classList.add('selected');

    //On click
    imagesRef.addEventListener('click', (e) => {
        //Deselect old indexShower
        indexShowers[selectedImageI].classList.remove('selected');

        selectedImageI++;

        if (selectedImageI >= ammOfImages)
            selectedImageI = 0; //Loop around

        const selectedImage = images[selectedImageI];
        imagesRef.scrollTo(
            {
                left: selectedImage.offsetLeft,
                behavior: 'smooth'
            });

        //Select new indexShower
        indexShowers[selectedImageI].classList.add('selected');
    });
}