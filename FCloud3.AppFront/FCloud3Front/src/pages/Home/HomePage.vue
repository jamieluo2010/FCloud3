@@
 <script setup lang="ts">
 import Search from '@/components/Search.vue';
 import WikiCenteredHomePage from './WikiCenteredHomePage.vue';
 import { injectApi } from '@/provides';
 import { useWikiParsingRoutesJump } from '../WikiParsing/routes/routesJump';
 import Footer from '@/components/Footer.vue'
 import SideBar from '@/components/SideBar.vue';
 import CreateWiki from '@/components/Wiki/CreateWiki.vue';
 import { onMounted, ref, useTemplateRef } from 'vue';
 import { guideInfo } from '@/utils/guideInfo';
 import { RouterLink } from 'vue-router';
 import Greeting from '@/components/Greeting.vue';
 import CurrencyManager from '@/components/CurrencyManager.vue';
+import TransportManager from '@/components/TransportManager.vue';
@@
     <WikiCenteredHomePage>
     </WikiCenteredHomePage>
     <CurrencyManager />
+    <TransportManager />
     <Footer></Footer>
 </template>
 
 <style scoped>
