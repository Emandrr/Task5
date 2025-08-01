
let take = 10;
let isLoading = false;
let hasMoreData = true;
let currentLanguage = '';
let currentBooks = window.currentBooks || [];
let allLoadedBooks = [];
$(document).ready(function () {
    currentLanguage = $('#languageSelect').val();
    currentBooks = window.currentBooks || [];
    allLoadedBooks = [...currentBooks];
    bindInputHandlers();
    bindEventHandlers();
    updateStickyHeaderPosition();
});

function getInitialBooks() {
    return JSON.parse($('#items-table-body').attr('data-books') || '[]');
}

function validateReviewValue(value) {
    let num = parseFloat(value);
    if (isNaN(num)) return 0;
    return Math.max(0, Math.min(20, num));
}

function showWarning(message) {
    const toast = $(
        `<div class="toast align-items-center text-white bg-warning border-0 position-fixed"
        style="top: 20px; right: 20px; z-index: 9999;">
      <div class="d-flex">
        <div class="toast-body">
          <i class="fas fa-exclamation-triangle me-2"></i>${message}
        </div>
        <button class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
      </div>
    </div>`
    ).appendTo('body');
    new bootstrap.Toast(toast[0], { autohide: true, delay: 3000 }).show();
    toast.on('hidden.bs.toast', () => toast.remove());
}

function updateStickyHeaderPosition() {
    let h = $('.sticky-controls').outerHeight();
    $('.sticky-header').css('top', h + 'px');
}

function renderMainRow(item) {
    return `
<tr class="item-row" data-item-id="${item.id}">
  <td>${item.id}</td><td>${item.isbn}</td>
  <td>${item.title}</td><td>${item.author}</td>
  <td>${item.publisher}</td>
</tr>`;
}

function renderReviewBlocks(authorRev, textRev) {
    let html = '';
    for (let i = 0; i < authorRev.length; i++) {
        html += `<div class="p-3 bg-white border rounded-md shadow-sm">
      <p class="text-sm text-gray-800">${authorRev[i]}</p>
      <p class="mt-2 text-xs text-gray-500">— ${textRev[i]}</p>
    </div>`;
    }
    return html;
}

function renderDetailsRow(item) {
    let reviews = item.authorRev ? renderReviewBlocks(item.authorRev, item.textRev) : '';
    return `
<tr class="details-row" id="details-${item.id}">
  <td colspan="5">
    <div class="book-details-container">
      <div class="book-layout">
        <div class="book-image-section">
          <div class="book-image-container">
            <img src="${item.image || ''}" alt="Обложка книги" />
          </div>
          <div class="book-likes">
            <span class="likes-count">${item.numberOfLikes}</span>
            <span class="like-icon">👍</span>
          </div>
        </div>
        <div class="book-info-section">
          <h2 class="book-title">${item.title}</h2>
          <p class="book-author">by ${item.author}</p>
          <p class="book-group">${item.publisher}</p>
          <h3 class="reviews-header">Review</h3>
          ${reviews}
        </div>
      </div>
    </div>
  </td>
</tr>`;
}

function renderItems(items) {
    let html = '';
    items.forEach(item => {
        html += renderMainRow(item);
        html += renderDetailsRow(item);
    });
    $('#items-table-body').append(html);
}

function updateData(likes, reviews) {
    $.post({
        url: '/Home/UpdateFilterData',
        contentType: 'application/json',
        data: JSON.stringify({ likes, reviews, books: currentBooks }),
        success: handleUpdateSuccess,
        complete: hideSpinner
    });
}

function handleUpdateSuccess(data) {
    if (data?.length) {
        currentBooks = data;
        $('#items-table-body').empty();
        renderItems(data);
        $('html, body').animate({ scrollTop: 0 }, 500);
        hasMoreData = true;
    }
}

function regenerateBooks() {
    isLoading = true;
    $('#loading-spinner').show();
    $.post({
        url: '/Home/TranslateBooks',
        contentType: 'application/json',
        data: JSON.stringify({ books: currentBooks, language: currentLanguage }),
        success: handleUpdateSuccess,
        complete: hideSpinner
    });
}

function regenerateTableWithoutSeed() {
    $('#loading-spinner').show();
    $('#items-table-body').empty();
    $.get('/Home/GenerateBooksWithoutSeed', function (res) {
        $('#seedInput').val(res.seed);
        renderItems(res.books);
    }).always(hideSpinner);
}

function regenerateTableWithSeed(seed) {
    $('#loading-spinner').show();
    $('#items-table-body').empty();
    $.get('/Home/GenerateBooksWithSeed', { seed }, function (res) {
        if (res?.length) renderItems(res);
    }).always(hideSpinner);
}

function loadMoreItems() {
    isLoading = true;
    $('#loading-spinner').show();

    $.ajax({
        url: '/Home/GetMoreItems',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(currentBooks),
        success: function (data) {
            if (data && data.length > 0) {
                renderItems(data);
                allLoadedBooks = allLoadedBooks.concat(data);
                currentBooks = allLoadedBooks;
            } else {
                hasMoreData = false;
            }
        },
        complete: function () {
            isLoading = false;
            $('#loading-spinner').hide();
        }
    });
}

function hideSpinner() {
    isLoading = false;
    $('#loading-spinner').hide();
}

function bindInputHandlers() {
    $(document).on('input change keyup', 'input[id*="review"]', e => {
        const $el = $(e.target);
        const val = $el.val();
        const validated = validateReviewValue(val);
        if (parseFloat(val) !== validated) {
            $el.val(validated.toFixed(1));
            if (parseFloat(val) > 20) showWarning('Максимальное значение для reviews: 20');
            if (parseFloat(val) < 0) showWarning('Минимальное значение для reviews: 0');
        }
    });

    $(document).on('blur', '#reviewInput', () => {
        let val = parseFloat($('#reviewInput').val());
        if (val > 20) $('#reviewInput').val('20.0');
        if (val < 0) $('#reviewInput').val('0.0');
    });
}

function bindEventHandlers() {
    $(window).on('resize', updateStickyHeaderPosition);
    $(window).on('scroll', () => {
        if ($(window).scrollTop() + $(window).height() >= $(document).height() - 100 && !isLoading && hasMoreData) {
            loadMoreItems();
        }
    });

    $('#items-table-body').on('click', '.item-row', e => {
        let id = $(e.currentTarget).data('item-id');
        $('#details-' + id).toggle();
    });

    $('#likesRange').on('input', function () {
        $('#likesValueDisplay').text(parseFloat(this.value).toFixed(1));
    }).on('change', function () {
        updateData(this.value, $('#reviewInput').val());
    });

    $('#reviewInput').on('keydown', function (e) {
        if (e.key === 'Enter') {
            updateData($('#likesRange').val(), this.value);
        }
    });

    $('#languageSelect').on('change', () => {
        currentLanguage = $('#languageSelect').val();
        regenerateBooks();
    });

    $('#randomSeedBtn').on('click', regenerateTableWithoutSeed);
    $('#seedInput').on('keydown', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            regenerateTableWithSeed($(this).val());
        }
    });
}
