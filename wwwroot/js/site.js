$(function () {
    //----------------------------------------------------Смена темы------------------------------------------------------------
    $('.theme').click(function () {
        $(this).toggleClass('yes');
        if ($(this).hasClass('yes') == true) {
            $('html').attr('data-theme-name', 'dark');

            $(this).removeClass('btn-outline-dark');
            $(this).addClass('btn-outline-light');
        }
        else {
            $('html').attr('data-theme-name', '');

            $(this).removeClass('btn-outline-light');
            $(this).addClass('btn-outline-dark');
        }
    });

    //----------------------------------------------------Добавление input для загрузки картинок------------------------------------------------------------
    $('.add-input-image').click(function () {
        $('<input asp-for="Image"  type="file"  name="uploadedFiles" class="form-control input-image" />').insertBefore('.add-input-image');
    })

    //----------------------------------------------------Активный элемент для карусели------------------------------------------------------------
    $('.carousel-item:first').addClass('active');
    $('.carousel-indicators-child:first').addClass('active');

    //----------------------------------------------------Удаление картинки в Edit------------------------------------------------------------

    $('span.image-edit-product').each(function () {
        $(this).click(function () {
            var imgstatus = $(this).next('img').attr('data-status');
            if (imgstatus == "save") {
                $(this).next('img').attr('data-status', 'del');
                $(this).next('img').css('opacity', '20%');
            } else {
                $(this).next('img').attr('data-status', 'save');
                $(this).next('img').css('opacity', '100%');
            }
        })
    })

    var mastosave = [];
    var mastodel = [];
    $('.save-to-edit').click(function () {
        $('img[data-status="save"]').each(function () {
            var name = $(this).attr('value');
            mastosave.push(name);
            var qq = JSON.stringify(mastosave);
            $('.saveimg').attr('value', qq);
        })
        $('img[data-status="del"]').each(function () {
            var name = $(this).attr('value');
            mastodel.push(name);
            var qq = JSON.stringify(mastodel);
            $('#delimg').attr('value', qq);
        })
    })

    //----------------------------------------------------Локальное хранилище данных и корзина------------------------------------------------------------

    var d = document,
        itemBox = d.querySelectorAll('.card'), // блок каждого товара
        cartCont = d.getElementById('cart_content'); // блок вывода данных корзины
    // Функция кроссбраузерной установка обработчика событий
    function addEvent(elem, type, handler) {
        if (elem !== null) {
            if (elem.addEventListener) {
                elem.addEventListener(type, handler, false);
            } else {
                elem.attachEvent('on' + type, function () { handler.call(elem); });
            }
            return false;
        }
        return false;
    }
    // Получаем данные из LocalStorage
    function getCartData() {
        return JSON.parse(localStorage.getItem('cart'));
    }
    // Записываем данные в LocalStorage
    function setCartData(o) {
        localStorage.setItem('cart', JSON.stringify(o));
        return false;
    }
    // Добавляем товар в корзину
    function addToCart(e) {
        this.disabled = true; // блокируем кнопку на время операции с корзиной
        $(this).css({ 'background-color': '#fc5335' });
        $(this).html("Купить ещё");
        var cartData = getCartData() || {}, // получаем данные корзины или создаём новый объект, если данных еще нет
            parentBox = this.parentNode, // родительский элемент кнопки "Добавить в корзину"
            itemId = this.getAttribute('data-id'), // ID товара
            itemTitle = $(this).parents(".card").find(".brand").html(), // название товара
            itemModel = $(this).parents(".card").find(".model").html(),
            itemPrice = $(this).parents(".card").find(".price").html(),
            itemImage = $(this).parents(".card").find(".card-img-top").attr("value"); // стоимость товара
        if (cartData.hasOwnProperty(itemId)) { // если такой товар уже в корзине, то добавляем +1 к его количеству
            cartData[itemId][3] += 1;

        } else { // если товара в корзине еще нет, то добавляем в объект
            cartData[itemId] = [itemTitle, itemModel, itemPrice, 1, itemImage];
        }
        if (!setCartData(cartData)) { // Обновляем данные в LocalStorage
            this.disabled = false; // разблокируем кнопку после обновления LS
        }
        cartSumInfo();
        return false;
    }
    // Устанавливаем обработчик события на каждую кнопку "Добавить в корзину"
    for (var i = 0; i < itemBox.length; i++) {
        addEvent(itemBox[i].querySelector('.cart-go'), 'click', addToCart);
    }
    // Открываем корзину со списком добавленных товаров
    function openCart(e) {
        var cartData = getCartData(), // вытаскиваем все данные корзины
            totalItems = '';
        // если что-то в корзине уже есть, начинаем формировать данные для вывода
        if (cartData !== null) {
            totalItems = '<table class="shopping_list">';/*<tr><th>Брэнд</th><th>Модель</th><th>Цена</th><th>Кол-во</th><th>Действия</th></tr>*/
            for (var items in cartData) {
                totalItems += '<tr id=' + items + '>';
                for (var i = 0; i < cartData[items].length; i++) {

                    if (i == 3) {
                        totalItems += `<td><button class="minus">-</button><span>${cartData[items][i]}</span><button class="plus">+</button></td>`;
                    } else if (i == 4) {
                        totalItems += '<td><img src="/Images/' + cartData[items][0] + '/' + cartData[items][1] + '/' + cartData[items][i] + '" class="cart-img" value="' + cartData[items][i] + '" alt="..."></td>';
                    } else {
                        totalItems += '<td>' + cartData[items][i] + '</td>';
                    }

                }
                totalItems += '<td><button class="btn btn-outline-primary del-row-cart">Удалить</button></td>';
                totalItems += '</tr>';
            }
            totalItems += '</table>';
            if (cartCont !== null) {
                cartCont.innerHTML = totalItems;
            }

        } else {
            // если в корзине пусто, то сигнализируем об этом
            $(".cart_content").html('В корзине пусто!');
        }
        return false;
    }
    /* Открыть корзину */
    openCart();


    /* Очистить корзину */
    addEvent(d.getElementById('clear_cart'), 'click', function (e) {
        localStorage.removeItem('cart');
        cartCont.innerHTML = 'Корзина очишена.';
        cartSumInfo();
    });

    /*Увеличить/уменьшить количество */
    $(".shopping_list").find('button').click(function () {
        itemId = $(this).parents("tr").attr('id');
        var cartData = getCartData();
        var num = cartData[itemId][3];
        if ($(this).hasClass("plus")) {
            cartData[itemId][3] += 1;
            setCartData(cartData);
            //location.reload();
            $(this).parents("tr").find("td").eq(3).find("span").empty();
            $(this).parents("tr").find("td").eq(3).find("span").append(cartData[itemId][3]);
            //history.go(0);
        }
        if ($(this).hasClass("minus")) {
            if (num > 1) {
                cartData[itemId][3] -= 1;
                setCartData(cartData);
                $(this).parents("tr").find("td").eq(3).find("span").empty();
                $(this).parents("tr").find("td").eq(3).find("span").append(cartData[itemId][3]);
                //location.reload();
                // history.go(0);
            }
        }
        cartSumInfo();
    });

    /* Удалить строку из корзины */
    $('.del-row-cart').click(function () {
        itemId = $(this).parents("tr").attr('id');
        var cartData = getCartData();
        delete cartData[itemId]; // удалили
        setCartData(cartData); // сохранили в локальном хранилище
        $(this).parents("tr").remove();
        //history.go(0);
        cartSumInfo();
    })


    // Количество элементов в корзине, включая повторяющиеся 
    function cartSumInfo() {
        var cartData = getCartData();
        var cartSum = 0;
        $(".cart-href").find("span").empty();
        if (cartData !== null) {
            for (var items in cartData) {
                cartSum += cartData[items][3];
            }

        }
        if (cartSum == 0) {
            $(".cart-href").find("span").html("");
            $(".clear-cart").hide();
            $(".arrange").hide();
            $(".cart_content").html('В корзине пусто!');
        } else {
            $(".cart-href").find("span").append(cartSum);
            $(".clear-cart").show();
            $(".arrange").show();
        }

    }
    cartSumInfo();

    //----------------------------------------------------Сортировка------------------------------------------------------------

    $(".sort").find("a").each(function () {
        var info2 = $(this).attr("href").slice(-3);
        console.log($(this));
        if (info2 == "esc") {
            $(this).find("span").empty();
            $(this).find("span").append("^");
        } else {
            $(this).find("span").empty();
            $(this).find("span").append("^");
            $(this).find("span").css("transform", "rotate(180deg)")
        }
        
    })
    //----------------------------------------------------Фильтрация------------------------------------------------------------
    
    $(".filter-to-index").click(function () {
        var massFilter = []
        $(this).parents(".filter").find(".checkbox-index").each(function () {
            if ($(this).is(':checked') == true) {
                var name = $(this).attr('value');
                massFilter.push(name);
            }
            var jsonFilter = JSON.stringify(massFilter);
            $('.filter-form-index').attr('value', jsonFilter);
        })
    })
    //----------------------------------------------------Модальное окно в Cart------------------------------------------------------------
   //Открыть/закрыть
    const btns = document.querySelectorAll('.arrange');
    const modalOverlay = document.querySelector('.modal-overlay ');
    const modals = document.querySelectorAll('.modal-iside');

    btns.forEach((el) => {
        el.addEventListener('click', (e) => {
            let path = e.currentTarget.getAttribute('data-path');

            modals.forEach((el) => {
                el.classList.remove('modal-iside--visible');
            });

            document.querySelector(`[data-target="${path}"]`).classList.add('modal-iside--visible');
            modalOverlay.classList.add('modal-overlay--visible');
        });
    });
    if (modalOverlay != null) {
        modalOverlay.addEventListener('click', (e) => {
        console.log(e.target);

        if (e.target == modalOverlay) {
            modalOverlay.classList.remove('modal-overlay--visible');
            modals.forEach((el) => {
                el.classList.remove('modal-iside--visible');
            });
        }
    });

    }
    
   
    //валидация содержимого модального окна

    $("form[name='ArrangeOrder']").validate({
        // Specify validation rules
        rules: {
            // The key name on the left side is the name attribute
            // of an input field. Validation rules are defined
            // on the right side
            UserFirstName: "required",
            UserSurName: "required",
            HomeAdress: "required",
            
        },
        // Specify validation error messages
        messages: {
            UserFirstName: "Please enter your firstname",
            UserSurName: "Please enter your lastname",
            HomeAdress: "Please enter a valid address",
            
        },
        // Make sure the form is submitted to the destination defined
        // in the "action" attribute of the form when valid
        submitHandler: function (form) {
            form.submit();
            //чистка корзины
            /*localStorage.removeItem('cart');
            cartCont.innerHTML = 'Корзина очишена.';
            cartSumInfo();*/
        }
    });
    // Составление данных о покупаемы товарах и запись их в json , передача в input с типом hidden
    $(".ArrangeOrder").click(function () {
        var cartData = getCartData()
        var arr = [];
        
        if (cartData != null) {
            for (var items in cartData) {
                var oldobj = {}
                var obj = {};
                var id = items;
                var quant = cartData[items][3];
                
                obj["id"] = id;
                obj["count"] = quant;
                oldobj["info"] = obj;
                arr.push(oldobj);
            }    
        }
        var newobj = {};
        newobj["History"] = arr;
        var jsonFilter = JSON.stringify(newobj);
        $("input[name='History']").attr('value', jsonFilter);
        
    })
})
