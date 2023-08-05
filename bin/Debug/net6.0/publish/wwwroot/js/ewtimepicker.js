/**
 * Create Time Picker (for ASP.NET Maker 2023)
 * @license Copyright (c) e.World Technology Limited. All rights reserved.
 */
ew.createTimePicker = function(formid, id, options) {
    if (id.includes("$rowindex$"))
        return;
    let $ = jQuery,
        el = ew.getElement(id, formid),
        $el = $(el),
        isInvalid = $el.hasClass("is-invalid"),
        format = options.timeFormat;
    if ($el.hasClass("ui-timepicker-input"))
        return;
    if (format) {
        // data is a JS Date object created by new Date(1970, 0, 2, hours, minutes, seconds, 0) with browser time zone
        // so we need to convert it back to UTC
        options.timeFormat = data => ew.formatDateTime(data.valueOf() / 1000 - data.getTimezoneOffset() * 60, format);
    }
    let inputGroup = $.isBoolean(options.inputGroup) ? options.inputGroup : true;
    delete(options.inputGroup);
    $el.timepicker(options).on("showTimepicker", function() {
        this.timepickerObj.list.width($el.outerWidth() - 2);
    }).on("focus", function() {
        $el.tooltip("hide").tooltip("disable");
    }).on("blur", function() {
        $el.tooltip("enable");
    });
    if (inputGroup) {
        let $btn = $('<button type="button"><i class="fa-regular fa-clock"></i></button>')
            .addClass("btn btn-default")
            .on("click", function() {
                $el.timepicker("show");
            });
        $el.wrap(`<div class="input-group${isInvalid ? " is-invalid" : ""}"></div>`).after($btn);
    }
}
