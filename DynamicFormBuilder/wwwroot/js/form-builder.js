$(function () {

    // API endpoint for fetching option values dynamically
    function getOptionValues(optionId) {
        return $.get('/api/options/' + optionId);
    }

    var fieldIndex = 0;

    // Create dynamic HTML for a new field using OptionValues from DB
    function buildFieldHtml(index, optionValues) {

        var html = `
            <div class="field-item" data-index="${index}" 
                style="border:1px solid #ddd;padding:10px;margin-top:10px;">

                <label>
                    Label:
                    <input type="text" class="fld-label form-control" placeholder="Enter label" />
                </label>

                <br />

                <label>
                    Required:
                    <input type="checkbox" class="fld-required" />
                </label>

                <br />

                <label>
                    Options:
                    <select class="fld-option form-control">
                        <option value="">-- Select --</option>
        `;

        optionValues.forEach(function (opt) {
            html += `<option value="${opt.optionValueId}">${opt.value}</option>`;
        });

        html += `
                    </select>
                </label>

                <br />

                <button class="remove-field btn" style="margin-top:10px;">Remove</button>

            </div>
        `;

        return html;
    }

    // Add More button click
    $("#addField").on("click", function (e) {
        e.preventDefault();

        // Load OptionValues from DB
        getOptionValues(window.defaultOptionId).then(function (data) {
            $("#fieldsContainer").append(buildFieldHtml(fieldIndex, data));
            fieldIndex++;
        });
    });

    // Remove dynamic field
    $("#fieldsContainer").on("click", ".remove-field", function (e) {
        e.preventDefault();
        $(this).closest('.field-item').remove();
    });

    // Submit button click
    $("#submitForm").on("click", function (e) {
        e.preventDefault();

        var title = $("#formTitle").val();

        if (!title || title.trim() === "") {
            alert("Form title is required.");
            return;
        }

        var fields = [];

        $(".field-item").each(function () {
            var label = $(this).find(".fld-label").val();
            var isRequired = $(this).find(".fld-required").is(":checked");
            var selectedOptionValue = $(this).find(".fld-option").val();

            if (!label || label.trim() === "") {
                alert("Each field must have a label.");
                return false;
            }

            fields.push({
                label: label,
                isRequired: isRequired,
                selectedOptionValueId: selectedOptionValue ? parseInt(selectedOptionValue) : null
            });
        });

        var payload = {
            title: title,
            fields: fields
        };

        $.ajax({
            url: "/api/form/save",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(payload),
            success: function (res) {
                alert("Form saved successfully. Form ID = " + res.formId);
                window.location = "/";
            },
            error: function (xhr) {
                alert("Error: " + xhr.responseText);
            }
        });

    });

});
