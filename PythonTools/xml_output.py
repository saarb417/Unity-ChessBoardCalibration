import xml.etree.ElementTree as ET

def xml_creator(distortion,matrix,ret,image_resolution,fps):
    # Create the root element
    root = ET.Element("camera_cfg")

    # Create and append child elements
    description = ET.SubElement(root, "description")
    description.text = "Unity Default Camera"

    type_elem = ET.SubElement(root, "type")
    type_elem.text = "camera"

    manufacturer = ET.SubElement(root, "manufacturer")
    manufacturer.text = "unity"

    model = ET.SubElement(root, "model")
    model.text = "camera"

    sn = ET.SubElement(root, "sn")
    sn.text = "x"

    version = ET.SubElement(root, "version")
    version.text = "0.0.0"

    versionDate = ET.SubElement(root, "versionDate")

    config = ET.SubElement(root, "config")

    framerate_fps = ET.SubElement(config, "framerate_fps")
    framerate_fps.text = str(fps)

    pixel_width_cols = ET.SubElement(config, "pixel_width_cols")
    pixel_width_cols.text = str(image_resolution[1])

    pixel_height_rows = ET.SubElement(config, "pixel_height_rows")
    pixel_height_rows.text = str(image_resolution[0])

    setup = ET.SubElement(config, "setup")
    setup.text = "monocular"

    lens_model = ET.SubElement(config, "lens_model")
    lens_model.text = "perspective"

    color_order = ET.SubElement(config, "color_order")
    color_order.text = "RGB"

    intr_params = ET.SubElement(config, "intr_params")

    rms = ET.SubElement(intr_params, "rms")
    rms.text = str(ret)  # Use the value of ret as RMS

    # Extracting camera matrix components
    fx = matrix[0][0]
    fy = matrix[1][1]
    cx = matrix[0][2]
    cy = matrix[1][2]

    fx_elem = ET.SubElement(intr_params, "fx")
    fx_elem.text = str(fx)

    fy_elem = ET.SubElement(intr_params, "fy")
    fy_elem.text = str(fy)

    cx_elem = ET.SubElement(intr_params, "cx")
    cx_elem.text = str(cx)

    cy_elem = ET.SubElement(intr_params, "cy")
    cy_elem.text = str(cy)

    # Add distortion coefficients here (k1, k2, k3, p1, p2)
    k1 = ET.SubElement(intr_params, "k1")
    k1.text = str(distortion[0][0])

    k2 = ET.SubElement(intr_params, "k2")
    k2.text = str(distortion[0][1])

    k3 = ET.SubElement(intr_params, "k3")
    k3.text = str(distortion[0][4])

    p1 = ET.SubElement(intr_params, "p1")
    p1.text = str(distortion[0][2])

    p2 = ET.SubElement(intr_params, "p2")
    p2.text = str(distortion[0][3])

    # Create an ElementTree object and save to an XML file
    tree = ET.ElementTree(root)
    tree.write("unity_cam.xml")

    print("XML file 'camera_config.xml' saved successfully.")
