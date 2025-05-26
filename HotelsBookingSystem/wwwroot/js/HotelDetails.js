document.addEventListener('DOMContentLoaded', function () {
    let currentImageId = 0;
    let currentImageIndex = 0;
    const carouselItems = document.querySelectorAll('.carousel-item');
    const totalImages = carouselItems.length;

    // Initialize carousel
    function initCarousel() {
        if (totalImages > 0) {
            // Hide all slides
            carouselItems.forEach(item => item.style.display = 'none');

            // Find the active slide or use first
            let activeIndex = 0;
            carouselItems.forEach((item, index) => {
                if (item.classList.contains('active')) {
                    activeIndex = index;
                }
            });

            // Show active slide
            currentImageIndex = activeIndex;
            carouselItems[currentImageIndex].style.display = 'block';
        }
    }

    // Call init on page load
    initCarousel();

    // Carousel navigation
    document.querySelector('.carousel-control.prev')?.addEventListener('click', function () {
        if (totalImages > 0) {
            currentImageIndex = (currentImageIndex - 1 + totalImages) % totalImages;
            updateCarousel();
        }
    });

    document.querySelector('.carousel-control.next')?.addEventListener('click', function () {
        if (totalImages > 0) {
            currentImageIndex = (currentImageIndex + 1) % totalImages;
            updateCarousel();
        }
    });

    function updateCarousel() {
        carouselItems.forEach(item => item.style.display = 'none');
        carouselItems[currentImageIndex].style.display = 'block';
    }

    // Add Hotel Image button
    document.getElementById('addHotelImageBtn')?.addEventListener('click', function () {
        document.getElementById('hotelImageModalTitle').textContent = 'Add Hotel Image';
        document.getElementById('imageId').value = '0';
        document.getElementById('hotelImageForm').reset();
        openModal(document.getElementById('hotelImageModal'));
    });

    // Set image as primary
    document.querySelectorAll('.image-action-btn.set-primary').forEach(button => {
        button.addEventListener('click', function () {
            const imageId = this.getAttribute('data-id');
            setImageAsPrimary(imageId);
        });
    });

    // Edit hotel image
    document.querySelectorAll('.image-action-btn.edit-image').forEach(button => {
        button.addEventListener('click', function () {
            const imageId = this.getAttribute('data-id');
            loadImageForEdit(imageId);
        });
    });

    // Delete hotel image
    document.querySelectorAll('.image-action-btn.delete-image').forEach(button => {
        button.addEventListener('click', function () {
            currentImageId = this.getAttribute('data-id');
            openModal(document.getElementById('deleteImageConfirmModal'));
        });
    });

    // Confirm delete image
    document.getElementById('confirmDeleteImageBtn')?.addEventListener('click', function () {
        if (currentImageId) deleteHotelImage(currentImageId);
    });

    // Hotel image form submission
    document.getElementById('hotelImageForm')?.addEventListener('submit', function (e) {
        e.preventDefault();
        if (validateImageForm()) {
            saveHotelImage();
        }
    });

    // Validate hotel image form
    function validateImageForm() {
        let isValid = true;

        // Reset validation
        document.querySelectorAll('#hotelImageForm .validation-message').forEach(el => {
            el.textContent = '';
            el.style.display = 'none';
        });

        // Validate Image (only for new images)
        if (document.getElementById('imageId').value === '0') {
            const imageInput = document.getElementById('hotelImage');
            if (!imageInput.files || imageInput.files.length === 0) {
                showError(imageInput, 'Hotel image is required');
                isValid = false;
            } else {
                // Validate file type
                const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg', 'image/gif'];
                const fileType = imageInput.files[0].type;

                if (!allowedTypes.includes(fileType)) {
                    showError(imageInput, 'Only JPG, PNG and GIF images are allowed');
                    isValid = false;
                }

                // Validate file size (max 5MB)
                const maxSize = 5 * 1024 * 1024; // 5MB in bytes
                if (imageInput.files[0].size > maxSize) {
                    showError(imageInput, 'Image must be less than 5MB');
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    // Load image for editing
    function loadImageForEdit(imageId) {
        document.getElementById('hotelImageModalTitle').textContent = 'Edit Hotel Image';
        document.getElementById('imageId').value = imageId;
        document.getElementById('hotelImageForm').reset();
        openModal(document.getElementById('hotelImageModal'));

        const formFields = document.querySelectorAll('#hotelImageForm input');
        formFields.forEach(field => field.disabled = true);

        fetch(`/HotelImage/GetImage/${imageId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                formFields.forEach(field => field.disabled = false);
                document.getElementById('hotelImage').required = false; 
                document.getElementById('isPrimary').checked = data.isPrimary || false;
                document.getElementById('caption').value = data.caption || '';
            })
            .catch(error => {
                alert('Failed to load image data: ' + error.message);
                formFields.forEach(field => field.disabled = false);
                closeAllModals();
            });
    }

    // Save hotel image
    function saveHotelImage() {
        const form = document.getElementById('hotelImageForm');
        const formData = new FormData(form);
        const imageId = document.getElementById('imageId').value;
        const url = imageId === "0" ? "/HotelImage/Create" : `/HotelImage/Update/${imageId}`;

        // Get token
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        // Show loading state
        const submitButton = form.querySelector('button[type="submit"]');
        if (submitButton) {
            submitButton.disabled = true;
            submitButton.textContent = 'Saving...';
        }

        fetch(url, {
            method: 'POST',
            body: formData,
            headers: { 'RequestVerificationToken': token }
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(errorData => {
                        throw new Error(errorData.message || 'Server error');
                    });
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    closeAllModals();
                    // Force a full page reload
                    window.location.href = updateUrlWithActiveTab(window.location.href, 'hotel-info');
                } else {
                    throw new Error(data.message || 'Failed to save image');
                }
            })
            .catch(error => {
                console.error('Save image error:', error);
                alert('Error: ' + error.message);

                // Reset submit button
                if (submitButton) {
                    submitButton.disabled = false;
                    submitButton.textContent = 'Save Image';
                }
            });
    }

    // Set image as primary
    function setImageAsPrimary(imageId) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (!token) {
            alert('Security token not found.');
            return;
        }

        fetch(`/HotelImage/SetAsPrimary/${imageId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        throw new Error(text || 'Unknown error');
                    });
                }
                // Reload the page to show updated primary image
                window.location.href = updateUrlWithActiveTab(window.location.href, 'hotel-info');
            })
            .catch(error => {
                console.error('Set primary image error:', error);
                alert('Error: ' + error.message);
            });
    }

    // Delete hotel image
    function deleteHotelImage(imageId) {
        if (!imageId) {
            alert('No image selected for deletion');
            return;
        }

        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (!token) {
            alert('Security token not found.');
            return;
        }

        // Show loading state
        const deleteBtn = document.getElementById('confirmDeleteImageBtn');
        if (deleteBtn) {
            deleteBtn.disabled = true;
            deleteBtn.textContent = 'Deleting...';
        }

        fetch(`/HotelImage/Delete/${imageId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        throw new Error(text || 'Unknown error');
                    });
                }
                closeAllModals();
                // Force a full page reload
                window.location.href = updateUrlWithActiveTab(window.location.href, 'hotel-info');
            })
            .catch(error => {
                console.error('Delete image error:', error);
                alert('Error: ' + error.message);

                // Reset delete button
                if (deleteBtn) {
                    deleteBtn.disabled = false;
                    deleteBtn.textContent = 'Delete';
                }
            });
    }
    let currentRoomId = 0;
    let currentServiceId = 0;

    // Get active tab from URL parameter or set default
    const urlParams = new URLSearchParams(window.location.search);
    const activeTab = urlParams.get('activeTab') || 'hotel-info';

    // Tab functionality
    function activateTab(tabId) {
        document.querySelectorAll('.tab-button').forEach(btn => btn.classList.remove('active'));
        document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));

        const tabButton = document.querySelector(`.tab-button[data-tab="${tabId}"]`);
        if (tabButton) {
            tabButton.classList.add('active');
            document.getElementById(tabId).classList.add('active');
        }
    }

    // Set the active tab on page load
    activateTab(activeTab);

    // Handle tab clicks
    document.querySelectorAll('.tab-button').forEach(button => {
        button.addEventListener('click', function () {
            const tabId = this.getAttribute('data-tab');
            activateTab(tabId);

            // Update URL with active tab parameter without reloading page
            const url = new URL(window.location.href);
            url.searchParams.set('activeTab', tabId);
            window.history.pushState({}, '', url);
        });
    });

    // Modal functions
    function openModal(modal) {
        modal.style.display = 'block';
        // Reset validation when opening modal
        resetValidation(modal);
    }

    function closeAllModals() {
        document.querySelectorAll('.modal').forEach(modal => modal.style.display = 'none');
    }

    // Reset validation for a form
    function resetValidation(modal) {
        const form = modal.querySelector('form');
        if (form) {
            form.reset();
            form.querySelectorAll('.form-control').forEach(el => {
                el.classList.remove('is-invalid');
            });
            form.querySelectorAll('.validation-message').forEach(el => {
                el.textContent = '';
                el.style.display = 'none';
            });
        }
    }

    // Show error message for a field
    function showError(inputElement, message) {
        inputElement.classList.add('is-invalid');
        const validationSpan = document.getElementById(inputElement.id + 'Validation');
        if (validationSpan) {
            validationSpan.textContent = message;
            validationSpan.style.display = 'block';
        } else {
            // If validation span doesn't exist, create one
            const parent = inputElement.parentElement;
            const span = document.createElement('span');
            span.id = inputElement.id + 'Validation';
            span.className = 'validation-message text-danger';
            span.textContent = message;
            span.style.display = 'block';
            parent.appendChild(span);
        }
    }

    // Close modals with buttons
    document.querySelectorAll('.close-modal, .cancel-btn').forEach(button => {
        button.addEventListener('click', closeAllModals);
    });

    // Add new room
    document.getElementById('addRoomBtn')?.addEventListener('click', function () {
        document.getElementById('roomFormModalTitle').textContent = 'Add New Room';
        document.getElementById('roomId').value = '0';
        document.getElementById('roomForm').reset();

        // Show the image field, required for new rooms
        const imageField = document.getElementById('image-field');
        if (imageField) {
            imageField.style.display = 'block';
            document.getElementById('image').required = true;
        }

        openModal(document.getElementById('roomFormModal'));
    });

    // Add new service
    document.getElementById('addServiceBtn')?.addEventListener('click', function () {
        document.getElementById('serviceFormModalTitle').textContent = 'Add New Service';
        document.getElementById('serviceId').value = '0';
        document.getElementById('serviceForm').reset();
        openModal(document.getElementById('serviceFormModal'));
    });

    // Edit room buttons
    document.querySelectorAll('#rooms .action-btn.edit').forEach(button => {
        button.addEventListener('click', function () {
            const roomId = this.getAttribute('data-id');
            loadRoomForEdit(roomId);
        });
    });

    // Edit service buttons
    document.querySelectorAll('#services .action-btn.edit').forEach(button => {
        button.addEventListener('click', function () {
            const serviceId = this.getAttribute('data-id');
            loadServiceForEdit(serviceId);
        });
    });

    // Delete room buttons
    document.querySelectorAll('#rooms .action-btn.delete').forEach(button => {
        button.addEventListener('click', function () {
            currentRoomId = this.getAttribute('data-id');
            openModal(document.getElementById('deleteRoomConfirmModal'));
        });
    });

    // Delete service buttons
    document.querySelectorAll('#services .action-btn.delete').forEach(button => {
        button.addEventListener('click', function () {
            currentServiceId = this.getAttribute('data-id');
            openModal(document.getElementById('deleteServiceConfirmModal'));
        });
    });

    // Confirm delete room
    document.getElementById('confirmDeleteRoomBtn')?.addEventListener('click', function () {
        if (currentRoomId) deleteRoom(currentRoomId);
    });

    // Confirm delete service
    document.getElementById('confirmDeleteServiceBtn')?.addEventListener('click', function () {
        if (currentServiceId) deleteService(currentServiceId);
    });

    // Room form submission with validation
    document.getElementById('roomForm')?.addEventListener('submit', function (e) {
        e.preventDefault();
        if (validateRoomForm()) {
            saveRoom();
        }
    });

    // Service form submission with validation 
    document.getElementById('serviceForm')?.addEventListener('submit', function (e) {
        e.preventDefault();
        if (validateServiceForm()) {
            saveService();
        }
    });

    // Room number validation check on input change
    const roomNumberField = document.getElementById('RoomNumber');
    const hotelSelect = document.getElementById('HotelId');

    if (roomNumberField && hotelSelect) {
        roomNumberField.addEventListener('change', function () {
            validateRoomNumber();
        });

        hotelSelect.addEventListener('change', function () {
            if (roomNumberField.value) {
                validateRoomNumber();
            }
        });
    }

    // Function to validate room number uniqueness
    function validateRoomNumber() {
        const roomNumber = document.getElementById('RoomNumber').value;
        const hotelId = document.getElementById('HotelId').value;
        const roomId = document.getElementById('roomId').value;

        if (!roomNumber || !hotelId) return;

        fetch(`/Room/CheckRoomNumber?hotelId=${hotelId}&roomNumber=${roomNumber}&roomId=${roomId}`)
            .then(response => response.json())
            .then(data => {
                if (data.exists) {
                    showError(document.getElementById('RoomNumber'), 'This room number already exists in this hotel');
                } else {
                    document.getElementById('RoomNumber').classList.remove('is-invalid');
                    const validationSpan = document.getElementById('RoomNumberValidation');
                    if (validationSpan) {
                        validationSpan.textContent = '';
                        validationSpan.style.display = 'none';
                    }
                }
            });
    }

    // Room form validation
    function validateRoomForm() {
        let isValid = true;

        // Reset validation
        document.querySelectorAll('#roomForm .form-control').forEach(el => {
            el.classList.remove('is-invalid');
        });
        document.querySelectorAll('#roomForm .validation-message').forEach(el => {
            el.textContent = '';
            el.style.display = 'none';
        });

        // Validate Hotel ID
        const hotelId = document.getElementById('HotelId');
        if (hotelId && (!hotelId.value || hotelId.value == "0")) {
            showError(hotelId, 'Please select a hotel');
            isValid = false;
        }

        // Validate Room Number
        const roomNumber = document.getElementById('RoomNumber');
        if (!roomNumber.value.trim()) {
            showError(roomNumber, 'Room number is required');
            isValid = false;
        }

        // Validate Type
        const type = document.getElementById('Type');
        if (!type.value) {
            showError(type, 'Please select a room type');
            isValid = false;
        }

        // Validate Number of Beds
        const beds = document.getElementById('NumberOfBeds');
        if (!beds.value || beds.value < 1) {
            showError(beds, 'Please enter a valid number (minimum 1)');
            isValid = false;
        }

        // Validate Price
        const price = document.getElementById('PricePerNight');
        if (!price.value || price.value <= 0) {
            showError(price, 'Please enter a valid price');
            isValid = false;
        }

        // Validate Status
        const status = document.getElementById('RoomStatus');
        if (!status.value) {
            showError(status, 'Please select a status');
            isValid = false;
        }

        // Validate Image (only for new rooms)
        if (document.getElementById('roomId').value === '0') {
            const image = document.getElementById('image');
            if (!image.files || image.files.length === 0) {
                showError(image, 'Room image is required');
                isValid = false;
            } else {
                // Validate file type
                const allowedTypes = ['image/jpeg', 'image/png', 'image/jpg', 'image/gif'];
                const fileType = image.files[0].type;

                if (!allowedTypes.includes(fileType)) {
                    showError(image, 'Only JPG, PNG and GIF images are allowed');
                    isValid = false;
                }

                // Validate file size (max 5MB)
                const maxSize = 5 * 1024 * 1024; // 5MB in bytes
                if (image.files[0].size > maxSize) {
                    showError(image, 'Image must be less than 5MB');
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    // Service form validation
    function validateServiceForm() {
        let isValid = true;

        // Reset validation
        document.querySelectorAll('#serviceForm .form-control').forEach(el => {
            el.classList.remove('is-invalid');
        });
        document.querySelectorAll('#serviceForm .validation-message').forEach(el => {
            el.textContent = '';
            el.style.display = 'none';
        });

        // Validate Service Name
        const serviceName = document.getElementById('ServiceName');
        if (!serviceName.value.trim()) {
            showError(serviceName, 'Service name is required');
            isValid = false;
        } else if (serviceName.value.trim().length < 2) {
            showError(serviceName, 'Name must be at least 2 characters');
            isValid = false;
        }

        // Validate Description (optional but with max length)
        const description = document.getElementById('ServiceDescription');
        if (description.value.length > 500) {
            showError(description, 'Description cannot exceed 500 characters');
            isValid = false;
        }

        // Validate Price
        const price = document.getElementById('ServicePrice');
        if (!price.value || price.value <= 0) {
            showError(price, 'Please enter a valid price (greater than 0)');
            isValid = false;
        } else if (price.value > 10000) {
            showError(price, 'Price cannot exceed $10,000');
            isValid = false;
        }

        return isValid;
    }

    // Load room for editing
    function loadRoomForEdit(roomId) {
        document.getElementById('roomFormModalTitle').textContent = 'Edit Room';
        document.getElementById('roomId').value = roomId;
        document.getElementById('roomForm').reset();
        openModal(document.getElementById('roomFormModal'));

        // Hide image field initially while loading data
        const imageField = document.getElementById('image-field');
        if (imageField) {
            imageField.style.display = 'none';
            document.getElementById('image').required = false;
        }

        const formFields = document.querySelectorAll('#roomForm input, #roomForm select, #roomForm textarea');
        formFields.forEach(field => field.disabled = true);

        fetch(`/Room/GetRoom/${roomId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                formFields.forEach(field => field.disabled = false);
                document.getElementById('RoomNumber').value = data.roomNumber || '';
                document.getElementById('Type').value = data.type || '';
                document.getElementById('NumberOfBeds').value = data.numberOfBeds || '';
                document.getElementById('PricePerNight').value = data.pricePerNight || '';
                document.getElementById('RoomDescription').value = data.description || '';
                document.getElementById('RoomStatus').value = data.status || '';

                if (document.getElementById('HotelId')) {
                    document.getElementById('HotelId').value = data.hotelId || '';
                }

                // Show image field for edit mode but make it optional
                if (imageField) {
                    imageField.style.display = 'block';
                    document.getElementById('image').required = false;
                }
            })
            .catch(error => {
                alert('Failed to load room data: ' + error.message);
                formFields.forEach(field => field.disabled = false);
                closeAllModals();
            });
    }

    // Load service for editing
    function loadServiceForEdit(serviceId) {
        document.getElementById('serviceFormModalTitle').textContent = 'Edit Service';
        document.getElementById('serviceId').value = serviceId;
        document.getElementById('serviceForm').reset();
        openModal(document.getElementById('serviceFormModal'));

        const formFields = document.querySelectorAll('#serviceForm input, #serviceForm textarea');
        formFields.forEach(field => field.disabled = true);

        fetch(`/Service/GetService/${serviceId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                formFields.forEach(field => field.disabled = false);
                document.getElementById('ServiceName').value = data.name || '';
                document.getElementById('ServiceDescription').value = data.description || '';
                document.getElementById('ServicePrice').value = data.price || '';
            })
            .catch(error => {
                alert('Failed to load service data: ' + error.message);
                formFields.forEach(field => field.disabled = false);
                closeAllModals();
            });
    }

    // Save room (create or update)
    function saveRoom() {
        const form = document.getElementById('roomForm');
        const formData = new FormData(form);
        const roomId = document.getElementById('roomId').value;
        const url = roomId === "0" ? "/Room/Create" : `/Room/Update/${roomId}`;

        // Get token
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        // Show loading state or disable submit button
        const submitButton = form.querySelector('button[type="submit"]');
        if (submitButton) {
            submitButton.disabled = true;
            submitButton.textContent = 'Saving...';
        }

        fetch(url, {
            method: 'POST',
            body: formData,
            headers: { 'RequestVerificationToken': token }
        })
            .then(response => {
                if (!response.ok) {
                    return response.json().then(errorData => {
                        throw new Error(errorData.message || 'Server error');
                    });
                }
                return response.json();
            })
            .then(data => {
                if (data.success) {
                    closeAllModals();
                    // Force a full page reload
                    window.location.href = updateUrlWithActiveTab(window.location.href, 'rooms');
                } else {
                    throw new Error(data.message || 'Failed to save room');
                }
            })
            .catch(error => {
                console.error('Save room error:', error);
                alert('Error: ' + error.message);

                // Reset submit button
                if (submitButton) {
                    submitButton.disabled = false;
                    submitButton.textContent = 'Save Room';
                }
            });
    }

    // Save service (create or update)
    function saveService() {
        const form = document.getElementById('serviceForm');
        const formData = new FormData(form);
        const serviceId = document.getElementById('serviceId').value;
        const url = serviceId === "0" ? "/Service/Create" : `/Service/Update/${serviceId}`;

        // Get token
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        // Show loading state or disable submit button
        const submitButton = form.querySelector('button[type="submit"]');
        if (submitButton) {
            submitButton.disabled = true;
            submitButton.textContent = 'Saving...';
        }

        // Log what we're sending (for debugging)
        console.log("Service form data:");
        for (let pair of formData.entries()) {
            console.log(pair[0] + ': ' + pair[1]);
        }

        fetch(url, {
            method: 'POST',
            body: formData,
            headers: { 'RequestVerificationToken': token }
        })
            .then(response => {
                // First log the raw response for debugging
                return response.text().then(text => {
                    console.log("Raw server response:", text);

                    // Now try to parse as JSON if possible
                    try {
                        const data = JSON.parse(text);
                        if (!response.ok) {
                            throw new Error(data.message || 'Server error');
                        }
                        return data;
                    } catch (e) {
                        // If can't parse as JSON, handle the error
                        if (!response.ok) {
                            throw new Error('Server error: ' + response.status);
                        }
                        throw new Error('Invalid JSON response from server');
                    }
                });
            })
            .then(data => {
                if (data.success) {
                    closeAllModals();
                    // Force a full page reload
                    window.location.href = updateUrlWithActiveTab(window.location.href, 'services');
                } else {
                    throw new Error(data.message || 'Failed to save service');
                }
            })
            .catch(error => {
                console.error('Save service error:', error);
                alert('Error: ' + error.message);

                // Reset submit button
                if (submitButton) {
                    submitButton.disabled = false;
                    submitButton.textContent = 'Save Service';
                }
            });
    }

    // Delete room
    function deleteRoom(roomId) {
        if (!roomId) {
            alert('No room selected for deletion');
            return;
        }

        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (!token) {
            alert('Security token not found.');
            return;
        }

        // Show loading state
        const deleteBtn = document.getElementById('confirmDeleteRoomBtn');
        if (deleteBtn) {
            deleteBtn.disabled = true;
            deleteBtn.textContent = 'Deleting...';
        }

        fetch(`/Room/Delete/${roomId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        throw new Error(text || 'Unknown error');
                    });
                }
                closeAllModals();
                // Force a full page reload
                window.location.href = updateUrlWithActiveTab(window.location.href, 'rooms');
            })
            .catch(error => {
                console.error('Delete room error:', error);
                alert('Error: ' + error.message);

                // Reset delete button
                if (deleteBtn) {
                    deleteBtn.disabled = false;
                    deleteBtn.textContent = 'Delete';
                }
            });
    }

    // Delete service
    function deleteService(serviceId) {
        if (!serviceId) {
            alert('No service selected for deletion');
            return;
        }

        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (!token) {
            alert('Security token not found.');
            return;
        }

        // Show loading state
        const deleteBtn = document.getElementById('confirmDeleteServiceBtn');
        if (deleteBtn) {
            deleteBtn.disabled = true;
            deleteBtn.textContent = 'Deleting...';
        }

        fetch(`/Service/Delete/${serviceId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            }
        })
            .then(response => {
                if (!response.ok) {
                    return response.text().then(text => {
                        throw new Error(text || 'Unknown error');
                    });
                }
                closeAllModals();
                // Force a full page reload
                window.location.href = updateUrlWithActiveTab(window.location.href, 'services');
            })
            .catch(error => {
                console.error('Delete service error:', error);
                alert('Error: ' + error.message);

                // Reset delete button
                if (deleteBtn) {
                    deleteBtn.disabled = false;
                    deleteBtn.textContent = 'Delete';
                }
            });
    }

    // Helper function to update URL with active tab
    function updateUrlWithActiveTab(url, tab) {
        const urlObj = new URL(url);
        urlObj.searchParams.set('activeTab', tab);
        return urlObj.toString();
    }
});



// Add this code to your HotelDetails.js file after the document.addEventListener('DOMContentLoaded', function () { line

// Hotel Image Carousel Variables
